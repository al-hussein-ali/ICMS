using System;
using System.Collections.Generic;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class LocalizationHelper
    {
        public static string GetLocalizedValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            if (value.TrimStart().StartsWith("{"))
            {
                try
                {
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(value);
                    if (dict != null)
                    {
                        var lang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
                        if (dict.TryGetValue(lang, out var localized))
                            return localized;
                        if (dict.TryGetValue("en", out var enFallback))
                            return enFallback;
                        return dict.Values.FirstOrDefault() ?? value;
                    }
                }
                catch
                {
                    // Fallback
                }
            }
            return value;
        }

        public static string GetLocalizedValue(string? value, string lang)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            if (value.TrimStart().StartsWith("{"))
            {
                try
                {
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(value);
                    if (dict != null)
                    {
                        var targetLang = lang.Split('-')[0].ToLower();
                        if (dict.TryGetValue(targetLang, out var localized))
                            return localized;
                        if (dict.TryGetValue("en", out var enFallback))
                            return enFallback;
                        return dict.Values.FirstOrDefault() ?? value;
                    }
                }
                catch
                {
                    // Fallback
                }
            }
            return value;
        }
    }
}
