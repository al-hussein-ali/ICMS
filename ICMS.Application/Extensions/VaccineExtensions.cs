using ICMS.Application.DTOs.Vaccine;
using ICMS.Domain.Entites.Clinical;

namespace ICMS.Application.Extensions
{
    public static class VaccineExtensions
    {
        public static VaccineReadDto ToReadDto(this Vaccine v)
            => new(v.Id, v.VaccineName, v.VaccineCode, v.Description, v.IsActive, v.TotalDosages, v.Audience);

        public static VaccineDetailsDto ToDetailsDto(this Vaccine v)
            => new(v.Id, v.VaccineName, v.VaccineCode, v.Description, v.IsActive, v.TotalDosages);

        public static Vaccine ToDomain(this VaccineCreateDto dto)
            => Vaccine.Create(dto.VaccineName, dto.VaccineCode, dto.Description, dto.IsActive, dto.TotalDosages, dto.Audience);
    }
}
