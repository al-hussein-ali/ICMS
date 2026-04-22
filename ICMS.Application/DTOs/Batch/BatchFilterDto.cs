namespace ICMS.Application.DTOs.Batch
{
    public record BatchFilterDto(
        int? DoseId,
        DateOnly? ExpiryDate,
        string? CookNumber = null,
        string? VaccineName = null,
        bool IncludeInactive = false);
}
