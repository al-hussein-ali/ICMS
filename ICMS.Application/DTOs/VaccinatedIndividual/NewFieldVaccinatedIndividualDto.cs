using ICMS.Application.DTOs.Person;

namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record NewFieldVaccinatedIndividualDto(
        int DirectorateId, 
        int NeighborhoodId, 
        int? SubNeighborhoodId, 
        PersonCreateDto Person, 
        int DoseId,
        DateOnly VaccinationDate,
        string TakenIn,
        string? CorrelationId = null,
        int? FieldVisitId = null,
        string? Note = null);
}
