using System;

namespace ICMS.Application.DTOs.Dose
{
    public record DoseReadDto(int Id, int VaccineId, string VaccineName, string DoseName, byte DoseOrder, int RecommendedAgeInMonths);
}
