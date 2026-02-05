using System;

namespace ICMS.Application.DTOs.Vaccine
{
    public record VaccineReadDto(int Id, string VaccineName, string VaccineCode, bool IsActive, byte TotalDosages);
}
