using ICMS.Application.DTOs.Dose;
using ICMS.Domain.Entites.Clinical;

namespace ICMS.Application.Extensions
{
    public static class DoseExtensions
    {
        public static DoseReadDto ToReadDto(this Dose d)
            => new(d.Id, d.VaccineId, LocalizationHelper.GetLocalizedValue(d.Vaccine?.VaccineName) ?? "N/A", LocalizationHelper.GetLocalizedValue(d.DoseName), d.DoseOrder, d.RecommendedAgeInWeeks, d.RecommendedAgeGroup, d.IsPrimary, d.Notes);

        public static DoseDetailsDto ToDetailsDto(this Dose d)
            => new(d.Id, d.VaccineId, LocalizationHelper.GetLocalizedValue(d.DoseName), d.DoseOrder, d.RecommendedAgeInWeeks, d.RecommendedAgeGroup, d.IsPrimary, d.Notes);

        public static Dose ToDomain(this DoseCreateDto dto)
            => Dose.Create(dto.VaccineId, dto.DoseName, dto.DoseOrder, dto.RecommendedAgeInWeeks, dto.RecommendedAgeGroup, dto.IsPrimary, dto.Notes);
    }
}
