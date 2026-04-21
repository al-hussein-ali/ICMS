using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchDetailsDto(
        int Id,
        string BatchName,
        int DoseId,
        int UserId,
        DateOnly CreationDate,
        DateOnly ExpiryDate,
        int TotalQuantity,
        string CountryOfOrigin,
        string CookNumber,
        string? Notes);
}
