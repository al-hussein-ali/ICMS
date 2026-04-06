using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Vaccine
{
    public record VaccineReadDto(int Id, string VaccineName, string VaccineCode, string? Description, bool IsActive, byte TotalDosages, TargetAudience Audience);
}
