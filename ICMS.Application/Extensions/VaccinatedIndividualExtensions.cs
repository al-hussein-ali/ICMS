using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class VaccinatedIndividualExtensions
    {
        public static VaccinatedIndividualReadDto ToReadDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.Directorate, vi.Area, vi.Neighborhood, vi.PersonId);

        public static VaccinatedIndividualDetailsDto ToDetailsDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.Directorate, vi.Area, vi.Neighborhood,vi.Person.ToReadDto());

        public static VaccinatedIndividual ToDomain(this VaccinatedIndividualCreateDto dto)
            => VaccinatedIndividual.Create(dto.Directorate,dto.Area,dto.Neighborhood); 
    }
}
