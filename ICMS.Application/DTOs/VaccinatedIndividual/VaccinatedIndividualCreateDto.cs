using ICMS.Application.DTOs.Person;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualCreateDto(int DirectorateId, int NeighborhoodId, int? SubNeighborhoodId, int? UserId, PersonCreateDto? PersonCreateDto, int? PersonId, DateOnly? RegistrationDate = null);
}
