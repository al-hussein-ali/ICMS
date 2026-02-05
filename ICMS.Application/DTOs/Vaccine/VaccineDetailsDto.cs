using System;

namespace ICMS.Application.DTOs.Vaccine
{
    public record VaccineDetailsDto(int Id, string VaccineName, string VaccineCode, string? Description, bool IsActive, byte TotalDosages);
}
