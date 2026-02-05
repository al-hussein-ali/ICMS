using ICMS.Application.DTOs.Dose;
using ICMS.Domain.Entites;

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
