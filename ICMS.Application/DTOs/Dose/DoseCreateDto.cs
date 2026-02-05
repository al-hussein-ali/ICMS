using System;

namespace ICMS.Application.DTOs.Dose
{
    public record DoseCreateDto(int VaccineId, string DoseName, byte DoseOrder, int RecommendedAgeInMonths, string RecommendedAgeGroup, string? Notes);
}
