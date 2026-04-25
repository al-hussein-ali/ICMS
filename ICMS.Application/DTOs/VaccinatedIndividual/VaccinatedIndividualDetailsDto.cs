using ICMS.Application.DTOs.Person;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualDetailsDto(int Id, string CardNumber, int DirectorateId, string DirectorateName, int NeighborhoodId, string NeighborhoodName, int? SubNeighborhoodId, int PersonId, int? UserId, PersonReadDto Person);
}
