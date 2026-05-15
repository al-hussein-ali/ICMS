using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;

        public SystemSettingsController(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _settingService.GetAllSettingsAsync();
            return Ok(settings);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var setting = await _settingService.GetSettingByKeyAsync(key);
            if (setting == null) return NotFound();
            return Ok(setting);
        }

        [HttpPut("{key}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateSettingRequest request)
        {
            var result = await _settingService.UpdateSettingAsync(key, request.Value);
            if (!result) return NotFound();
            return Ok();
        }

        public class UpdateSettingRequest
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}
