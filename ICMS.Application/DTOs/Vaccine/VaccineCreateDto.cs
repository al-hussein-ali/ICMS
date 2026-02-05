using System;

namespace ICMS.Application.DTOs.Vaccine
{
    public record VaccineCreateDto(string VaccineName, string VaccineCode, string? Description, bool IsActive, byte TotalDosages);
}
