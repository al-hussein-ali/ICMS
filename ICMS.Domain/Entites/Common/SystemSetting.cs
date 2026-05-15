using ICMS.Domain.Entites.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Common
{
    public class SystemSetting : BaseEntity<int>
    {
        public string Key { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string DataType { get; private set; } = "string"; // string, int, bool, time

        private SystemSetting() { }

        public static SystemSetting Create(string key, string value, string category, string description = "", string dataType = "string")
        {
            return new SystemSetting
            {
                Key = key,
                Value = value,
                Category = category,
                Description = description,
                DataType = dataType
            };
        }

        public void UpdateValue(string newValue)
        {
            Value = newValue;
        }
    }
}
