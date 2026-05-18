using ICMS.Application.DTOs.Dose;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using System.Collections.Generic;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class DoseExtensions
    {
        private static string GetLocalizedDoseName(string doseName)
        {
            if (string.IsNullOrWhiteSpace(doseName)) return string.Empty;
            if (doseName.TrimStart().StartsWith("{"))
            {
                try
                {
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(doseName);
                    if (dict != null)
                    {
                        var lang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
                        if (dict.TryGetValue(lang, out var localized))
                            return localized;
                        if (dict.TryGetValue("en", out var enFallback))
                            return enFallback;
                        return dict.Values.FirstOrDefault() ?? doseName;
                    }
                }
                catch
                {
                    // Fallback
                }
            }
            return doseName;
        }

        public static DoseReadDto ToReadDto(this Dose d)
            => new(d.Id, d.VaccineId, d.Vaccine?.VaccineName ?? "N/A", GetLocalizedDoseName(d.DoseName), d.DoseOrder, d.RecommendedAgeInWeeks, d.RecommendedAgeGroup, d.IsPrimary, d.Notes);

        public static DoseDetailsDto ToDetailsDto(this Dose d)
            => new(d.Id, d.VaccineId, GetLocalizedDoseName(d.DoseName), d.DoseOrder, d.RecommendedAgeInWeeks, d.RecommendedAgeGroup, d.IsPrimary, d.Notes);

        public static Dose ToDomain(this DoseCreateDto dto)
            => Dose.Create(dto.VaccineId, dto.DoseName, dto.DoseOrder, dto.RecommendedAgeInWeeks, dto.RecommendedAgeGroup, dto.IsPrimary, dto.Notes);
    }
}
