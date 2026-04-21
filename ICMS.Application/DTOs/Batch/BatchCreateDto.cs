using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchCreateDto(int DoseId, string BatchName, DateOnly CreationDate, string CountryOfOrigin, string CookNumber, DateOnly ExpiryDate, int TotalQuantity, string? Notes);
}
