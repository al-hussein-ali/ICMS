using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Domain.Entites.Identity;

namespace ICMS.Application.Extensions
{
    public static class VaccinatedIndividualExtensions
    {
        public static VaccinatedIndividualReadDto ToReadDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.DirectorateId, vi.Directorate.Name, vi.NeighborhoodId, vi.Neighborhood.Name, vi.SubNeighborhoodId, vi.PersonId, vi.UserId, vi.Person.ToReadDto());

        public static VaccinatedIndividualDetailsDto ToDetailsDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.DirectorateId, vi.Directorate.Name, vi.NeighborhoodId, vi.Neighborhood.Name, vi.SubNeighborhoodId, vi.PersonId, vi.UserId, vi.Person.ToReadDto());
    }
}
