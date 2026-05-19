using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Common;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.ExternalServices
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly AppDbContext _context;

        public SystemSettingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync()
        {
            return await _context.SystemSettings.ToListAsync();
        }

        public async Task<SystemSetting?> GetSettingByKeyAsync(string key)
        {
            return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<bool> UpdateSettingAsync(string key, string value)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null)
            {
                setting = SystemSetting.Create(key, value, "System", "Auto-generated system setting", "string");
                await _context.SystemSettings.AddAsync(setting);
            }
            else
            {
                setting.UpdateValue(value);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T> GetValueAsync<T>(string key, T defaultValue)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null) return defaultValue;

            try
            {
                if (typeof(T) == typeof(bool))
                    return (T)(object)bool.Parse(setting.Value);
                if (typeof(T) == typeof(int))
                    return (T)(object)int.Parse(setting.Value);
                if (typeof(T) == typeof(string))
                    return (T)(object)setting.Value;

                return (T)Convert.ChangeType(setting.Value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
