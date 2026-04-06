namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualReadDto(int Id, string CardNumber, int DirectorateId, int NeighborhoodId, int? SubNeighborhoodId, int PersonId, int? UserId);
}
