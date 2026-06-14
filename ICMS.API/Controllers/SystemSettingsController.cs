using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.StaffRoles)]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;
        private readonly IRecurringJobManager _recurringJobManager;

        public SystemSettingsController(ISystemSettingService settingService, IRecurringJobManager recurringJobManager)
        {
            _settingService = settingService;
            _recurringJobManager = recurringJobManager;
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

            // Dynamically update Hangfire recurrent job schedule if the broadcast time changed
            if (key == "Advisory.DailyBroadcastTime")
            {
                string cron = "0 5 * * *"; 
                if (System.TimeSpan.TryParse(request.Value, out var ts))
                {
                    var utcTime = ts.Subtract(System.TimeSpan.FromHours(3));
                    if (utcTime < System.TimeSpan.Zero) utcTime = utcTime.Add(System.TimeSpan.FromHours(24));
                    cron = $"{utcTime.Minutes} {utcTime.Hours} * * *";
                }

                _recurringJobManager.AddOrUpdate<ICMS.Application.Interfaces.Services.IAdvisoryDispatchBackgroundService>(
                    "DailyHealthAdvisoryDispatcher",
                    service => service.DispatchPendingAdvisoriesAsync(default),
                    cron);
            }

            return Ok();
        }

        public class UpdateSettingRequest
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}
