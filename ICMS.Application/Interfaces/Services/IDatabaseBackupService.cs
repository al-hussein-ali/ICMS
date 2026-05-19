using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IDatabaseBackupService
    {
        Task<string> GetBackupPathAsync();
        Task SaveBackupPathAsync(string path);
        Task<BackupResultDto> RunBackupAsync(string? customPath = null);
        Task<RestoreResultDto> RestoreBackupAsync(System.IO.Stream fileStream);
    }

    public record BackupResultDto(string FileName, string FullPath, bool Success, string? ErrorMessage = null);
    public record RestoreResultDto(bool Success, string? ErrorMessage = null);
}
