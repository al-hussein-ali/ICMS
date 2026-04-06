namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record VaccinatedIndividualDetailsDto(int Id, string CardNumber, int DirectorateId, int NeighborhoodId, int? SubNeighborhoodId, int PersonId, int? UserId);
}
