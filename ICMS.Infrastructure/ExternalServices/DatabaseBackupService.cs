using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.ExternalServices
{
    public class DatabaseBackupService : IDatabaseBackupService
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly IConfiguration _configuration;

        public DatabaseBackupService(ISystemSettingService systemSettingService, IConfiguration configuration)
        {
            _systemSettingService = systemSettingService;
            _configuration = configuration;
        }

        public async Task<string> GetBackupPathAsync()
        {
            return await _systemSettingService.GetValueAsync("Backup.DefaultPath", string.Empty);
        }

        public async Task SaveBackupPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Backup path cannot be empty.");
            }

            // Strip double quotes, quotes, and command/shell control characters
            var sanitizedPath = path.Replace("\"", "").Replace("'", "").Replace(";", "").Replace("&", "").Replace("|", "").Replace("`", "").Replace("$", "");
            
            var allowedRoot = _configuration["BackupSettings:RootPath"] ?? @"C:\ICMSBackups";
            allowedRoot = Path.GetFullPath(allowedRoot);

            var resolvedPath = Path.GetFullPath(sanitizedPath);

            if (!resolvedPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Access denied. Backup path must be within {allowedRoot}.");
            }

            await _systemSettingService.UpdateSettingAsync("Backup.DefaultPath", resolvedPath);
        }

        public async Task<BackupResultDto> RunBackupAsync(string? customPath = null)
        {
            try
            {
                var targetPath = customPath;
                if (string.IsNullOrWhiteSpace(targetPath))
                {
                    targetPath = await GetBackupPathAsync();
                }

                if (string.IsNullOrWhiteSpace(targetPath))
                {
                    return new BackupResultDto(string.Empty, string.Empty, false, "Backup path is not set.");
                }

                // Strip double quotes, quotes, and command/shell control characters
                var sanitizedPath = targetPath.Replace("\"", "").Replace("'", "").Replace(";", "").Replace("&", "").Replace("|", "").Replace("`", "").Replace("$", "");
                
                var allowedRoot = _configuration["BackupSettings:RootPath"] ?? @"C:\ICMSBackups";
                allowedRoot = Path.GetFullPath(allowedRoot);

                var resolvedPath = Path.GetFullPath(sanitizedPath);

                if (!resolvedPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
                {
                    return new BackupResultDto(string.Empty, string.Empty, false, $"Access denied. Backup path must be within {allowedRoot}.");
                }

                // Ensure target directory exists
                if (!Directory.Exists(resolvedPath))
                {
                    Directory.CreateDirectory(resolvedPath);
                }

                // Get DB connection parameters
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return new BackupResultDto(string.Empty, string.Empty, false, "DefaultConnection string is empty or not configured.");
                }

                var connBuilder = new NpgsqlConnectionStringBuilder(connectionString);
                var host = connBuilder.Host ?? "localhost";
                var port = connBuilder.Port != 0 ? connBuilder.Port.ToString() : "5432";
                var database = connBuilder.Database ?? "ICMSDB";
                var username = connBuilder.Username ?? "postgres";
                var password = connBuilder.Password ?? string.Empty;

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"icms_backup_{timestamp}.sql";
                var fullPath = Path.Combine(resolvedPath, fileName);

                var pgDumpPath = GetPgDumpPath();

                var startInfo = new ProcessStartInfo
                {
                    FileName = pgDumpPath,
                    Arguments = $"-h {host} -p {port} -U {username} -F p -b -v -w -f \"{fullPath}\" {database}",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                if (!string.IsNullOrEmpty(password))
                {
                    startInfo.EnvironmentVariables["PGPASSWORD"] = password;
                }

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    var stderr = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        return new BackupResultDto(fileName, fullPath, false, $"pg_dump failed with exit code {process.ExitCode}: {stderr}");
                    }
                }

                return new BackupResultDto(fileName, fullPath, true);
            }
            catch (Exception ex)
            {
                return new BackupResultDto(string.Empty, string.Empty, false, ex.Message);
            }
        }

        public async Task<RestoreResultDto> RestoreBackupAsync(Stream fileStream)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"restore_{Guid.NewGuid()}.sql");
            try
            {
                using (var file = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    await fileStream.CopyToAsync(file);
                }

                // Get DB connection parameters
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return new RestoreResultDto(false, "DefaultConnection string is empty or not configured.");
                }

                var connBuilder = new NpgsqlConnectionStringBuilder(connectionString);
                var host = connBuilder.Host ?? "localhost";
                var port = connBuilder.Port != 0 ? connBuilder.Port.ToString() : "5432";
                var database = connBuilder.Database ?? "ICMSDB";
                var username = connBuilder.Username ?? "postgres";
                var password = connBuilder.Password ?? string.Empty;

                // Clean the schema to prevent conflicts
                await CleanSchemaAsync(connBuilder, database);

                var psqlPath = GetPsqlPath();

                var startInfo = new ProcessStartInfo
                {
                    FileName = psqlPath,
                    Arguments = $"-h {host} -p {port} -U {username} -d {database} -w -f \"{tempFilePath}\"",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                if (!string.IsNullOrEmpty(password))
                {
                    startInfo.EnvironmentVariables["PGPASSWORD"] = password;
                }

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    var stderr = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        return new RestoreResultDto(false, $"psql failed with exit code {process.ExitCode}: {stderr}");
                    }
                }

                // Clear Npgsql connection pools so subsequent database connections start fresh
                NpgsqlConnection.ClearAllPools();

                return new RestoreResultDto(true);
            }
            catch (Exception ex)
            {
                return new RestoreResultDto(false, ex.Message);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch { }
                }
            }
        }

        private async Task CleanSchemaAsync(NpgsqlConnectionStringBuilder connBuilder, string database)
        {
            using (var conn = new NpgsqlConnection(connBuilder.ConnectionString))
            {
                await conn.OpenAsync();
                
                // Terminate other connections
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                        SELECT pg_terminate_backend(pg_stat_activity.pid)
                        FROM pg_stat_activity
                        WHERE pg_stat_activity.datname = '{database}'
                          AND pid <> pg_backend_pid();";
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch { /* Ignore if permission is denied to terminate other backends */ }
                }

                // Drop and recreate schema public
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DROP SCHEMA public CASCADE; CREATE SCHEMA public; GRANT ALL ON SCHEMA public TO public;";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private string GetPsqlPath()
        {
            // Standard paths on Windows
            var pathsToCheck = new[]
            {
                @"C:\Program Files\PostgreSQL\17\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\16\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\15\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\14\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\13\bin\psql.exe"
            };

            foreach (var path in pathsToCheck)
            {
                if (File.Exists(path)) return path;
            }

            // Search PostgreSQL directory dynamically
            var pgDir = @"C:\Program Files\PostgreSQL";
            if (Directory.Exists(pgDir))
            {
                try
                {
                    var exeFiles = Directory.GetFiles(pgDir, "psql.exe", SearchOption.AllDirectories);
                    if (exeFiles.Length > 0)
                    {
                        return exeFiles[0];
                    }
                }
                catch
                {
                    // Ignore exceptions during file search
                }
            }

            // Default fallback
            return "psql";
        }

        private string GetPgDumpPath()
        {
            // Standard paths on Windows
            var pathsToCheck = new[]
            {
                @"C:\Program Files\PostgreSQL\17\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\16\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\15\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\14\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\13\bin\pg_dump.exe"
            };

            foreach (var path in pathsToCheck)
            {
                if (File.Exists(path)) return path;
            }

            // Search PostgreSQL directory dynamically
            var pgDir = @"C:\Program Files\PostgreSQL";
            if (Directory.Exists(pgDir))
            {
                try
                {
                    var exeFiles = Directory.GetFiles(pgDir, "pg_dump.exe", SearchOption.AllDirectories);
                    if (exeFiles.Length > 0)
                    {
                        return exeFiles[0];
                    }
                }
                catch
                {
                    // Ignore exceptions during file search
                }
            }

            // Default fallback
            return "pg_dump";
        }
    }
}
