using ICMS.Application.DTOs.Dose;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class DoseExtensions
    {
        public static DoseReadDto ToReadDto(this Dose d)
            => new(d.Id, d.VaccineId, d.Vaccine?.VaccineName ?? "N/A", d.DoseName, d.DoseOrder, d.RecommendedAgeInWeeks, d.RecommendedAgeGroup, d.IsPrimary, d.Notes);

        public static DoseDetailsDto ToDetailsDto(this Dose d)
            => new(d.Id, d.VaccineId, d.DoseName, d.DoseOrder, d.RecommendedAgeInWeeks, d.RecommendedAgeGroup, d.IsPrimary, d.Notes);

        public static Dose ToDomain(this DoseCreateDto dto)
            => Dose.Create(dto.VaccineId, dto.DoseName, dto.DoseOrder, dto.RecommendedAgeInWeeks, dto.RecommendedAgeGroup, dto.IsPrimary, dto.Notes);
    }
}
