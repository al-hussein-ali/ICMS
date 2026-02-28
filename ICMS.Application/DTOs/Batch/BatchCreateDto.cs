using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchCreateDto(int DoseId, int UserId, string CountryOfOrigin, string? CookNumber, DateOnly ExpiryDate, int TotalQuantity, string? Notes);
}
