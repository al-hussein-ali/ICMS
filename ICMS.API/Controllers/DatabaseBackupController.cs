using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class DatabaseBackupController : ControllerBase
    {
        private readonly IDatabaseBackupService _backupService;

        public DatabaseBackupController(IDatabaseBackupService backupService)
        {
            _backupService = backupService;
        }

        [HttpGet("path")]
        public async Task<IActionResult> GetBackupPath()
        {
            var path = await _backupService.GetBackupPathAsync();
            return Ok(new { Path = path });
        }

        [HttpPost("path")]
        public async Task<IActionResult> SaveBackupPath([FromBody] SaveBackupPathRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Path))
            {
                return BadRequest("Path cannot be empty.");
            }
            await _backupService.SaveBackupPathAsync(request.Path);
            return Ok();
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunBackup([FromBody] RunBackupRequest request)
        {
            var result = await _backupService.RunBackupAsync(request.CustomPath);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.ErrorMessage, FileName = result.FileName, FullPath = result.FullPath });
            }
            return Ok(result);
        }

        [HttpPost("restore")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> RestoreBackup([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No backup file selected.");
            }

            using (var stream = file.OpenReadStream())
            {
                var result = await _backupService.RestoreBackupAsync(stream);
                if (!result.Success)
                {
                    return BadRequest(new { Message = result.ErrorMessage });
                }
            }

            return Ok(new { Message = "Database restored successfully." });
        }

        public class SaveBackupPathRequest
        {
            public string Path { get; set; } = string.Empty;
        }

        public class RunBackupRequest
        {
            public string? CustomPath { get; set; }
        }
    }
}
