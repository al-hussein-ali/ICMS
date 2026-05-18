using ICMS.Application.DTOs.Vaccine;
using ICMS.Domain.Entites.Clinical;
using System.Collections.Generic;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class VaccineExtensions
    {
        private static string GetLocalizedValue(string? value)
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

        public static VaccineReadDto ToReadDto(this Vaccine v)
            => new(v.Id, GetLocalizedValue(v.VaccineName), v.VaccineCode, GetLocalizedValue(v.Description), v.IsActive, v.TotalDosages, v.MinEligibleAgeInMonths, v.MaxEligibleAgeInMonths, v.Audience);

        public static VaccineDetailsDto ToDetailsDto(this Vaccine v)
            => new(v.Id, GetLocalizedValue(v.VaccineName), v.VaccineCode, GetLocalizedValue(v.Description), v.IsActive, v.TotalDosages, v.MinEligibleAgeInMonths, v.MaxEligibleAgeInMonths, v.Audience);

        public static Vaccine ToDomain(this VaccineCreateDto dto)
            => Vaccine.Create(dto.VaccineName, dto.VaccineCode, dto.Description, dto.IsActive, dto.TotalDosages, dto.MinEligibleAgeInMonths, dto.MaxEligibleAgeInMonths, dto.Audience);
    }
}
