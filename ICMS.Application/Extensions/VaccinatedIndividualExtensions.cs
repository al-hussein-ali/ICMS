using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Domain.Entites.Identity;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class VaccinatedIndividualExtensions
    {
        public static VaccinatedIndividualReadDto ToReadDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.Directorate.GovernorateId, vi.Directorate.Governorate.Name, vi.DirectorateId, vi.Directorate.Name, vi.NeighborhoodId, vi.Neighborhood.Name, vi.SubNeighborhoodId, vi.PersonId, vi.UserId, vi.Person.ToReadDto(), vi.User?.UserName);

        public static VaccinatedIndividualDetailsDto ToDetailsDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.Directorate.GovernorateId, vi.Directorate.Governorate.Name, vi.DirectorateId, vi.Directorate.Name, vi.NeighborhoodId, vi.Neighborhood.Name, vi.SubNeighborhoodId, vi.PersonId, vi.UserId, vi.Person.ToReadDto(), vi.Schedules.Select(s => s.ToReadDto()).ToList(), vi.User?.UserName);
    }
}
