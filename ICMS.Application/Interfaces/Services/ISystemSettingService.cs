using ICMS.Domain.Entites.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface ISystemSettingService
    {
        Task<IEnumerable<SystemSetting>> GetAllSettingsAsync();
        Task<SystemSetting?> GetSettingByKeyAsync(string key);
        Task<bool> UpdateSettingAsync(string key, string value);
        Task<T> GetValueAsync<T>(string key, T defaultValue);
    }
}
