using ICMS.Application.DTOs.Person;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualReadDto(int Id, string CardNumber, int GovernorateId, string GovernorateName, int DirectorateId, string DirectorateName, int NeighborhoodId, string NeighborhoodName, int? SubNeighborhoodId, int PersonId, int? UserId, PersonReadDto Person, string? UserName);
}
