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
            => new(d.Id, d.VaccineId, d.DoseName, d.DoseOrder);

        public static DoseDetailsDto ToDetailsDto(this Dose d)
            => new(d.Id, d.VaccineId, d.DoseName, d.DoseOrder, d.Notes);

        public static Dose ToDomain(this DoseCreateDto dto)
            => Dose.Create(dto.VaccineId, dto.DoseName, dto.DoseOrder, dto.RecommendedAgeInMonths, dto.RecommendedAgeGroup, dto.Notes);
    }
}
