using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Domain.Entites.Identity;
using System;

namespace ICMS.Application.Extensions
{
    public static class VaccinatedIndividualExtensions
    {
        public static VaccinatedIndividualReadDto ToReadDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.DirectorateId, vi.NeighborhoodId, vi.SubNeighborhoodId, vi.PersonId, vi.UserId);

        public static VaccinatedIndividualDetailsDto ToDetailsDto(this VaccinatedIndividual vi)
            => new(vi.Id, vi.CardNumber, vi.DirectorateId, vi.NeighborhoodId, vi.SubNeighborhoodId, vi.PersonId, vi.UserId);

        public static VaccinatedIndividual ToDomain(this VaccinatedIndividualCreateDto dto)
            => VaccinatedIndividual.Create(dto.DirectorateId, dto.NeighborhoodId, dto.SubNeighborhoodId, dto.UserId);

        public static VaccinatedIndividual ToDomain(this NewFieldVaccinatedIndividualDto dto)
            => VaccinatedIndividual.Create(dto.DirectorateId, dto.NeighborhoodId, dto.SubNeighborhoodId);
 
    }
}
