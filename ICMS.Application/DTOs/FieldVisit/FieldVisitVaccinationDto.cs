namespace ICMS.Application.DTOs.FieldVisit
{
    /// <summary>
    /// DTO for a single vaccinated individual entry within a field visit.
    /// </summary>
    public record FieldVisitVaccinatedPersonDto(
        int Id,
        string FullName,
        string CardNumber,
        string PhoneNumber,
        List<string> DelayedDoseNames,
        List<FieldVisitWorkerDto> AdministeredBy
    );

    /// <summary>
    /// DTO for a field worker who administered vaccinations in a visit.
    /// </summary>
    public record FieldVisitWorkerDto(
        int Id,
        string FirstName,
        string LastName,
        string FullName,
        string Username
    );

    /// <summary>
    /// Summary DTO returned by GET /api/field-visits/{id}/vaccinations.
    /// Contains all vaccinated individuals and the field workers who administered
    /// the vaccinations in a single field visit.
    /// </summary>
    public record FieldVisitVaccinationsDto(
        int FieldVisitId,
        string CampaignName,
        DateOnly VisitDate,
        int TotalVaccinated,
        List<FieldVisitVaccinatedPersonDto> VaccinatedPersons,
        List<FieldVisitWorkerDto> AdministeredBy
    );
}
