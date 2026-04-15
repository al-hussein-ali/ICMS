namespace ICMS.Application.DTOs.VaccinatedIndividual
{
    public record UpdateFieldVisitIndividualDto(
        int IndividualId,
        int DoseId,
        int FieldVisitId,
        DateOnly VaccinationDate,
        string TakenIn,
        string? Note);
}
