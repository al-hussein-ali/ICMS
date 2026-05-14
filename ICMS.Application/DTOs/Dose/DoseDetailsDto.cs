using System;

namespace ICMS.Application.DTOs.Dose
{
    public record DoseDetailsDto(int Id, int VaccineId, string DoseName, byte DoseOrder, int RecommendedAgeInWeeks, string RecommendedAgeGroup, bool IsPrimary, string? Notes);
}
