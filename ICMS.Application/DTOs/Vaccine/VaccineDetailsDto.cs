using System;

namespace ICMS.Application.DTOs.Vaccine
{
    public record VaccineDetailsDto(int Id, string VaccineName, string VaccineCode, string? Description, bool IsActive, byte TotalDosages, int MinEligibleAgeInMonths, int MaxEligibleAgeInMonths, ICMS.Domain.Enums.TargetAudience Audience);
}
