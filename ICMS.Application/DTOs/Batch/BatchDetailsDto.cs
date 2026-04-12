using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchDetailsDto(int Id, int DoseId, int UserId, DateOnly ExpiryDate, int TotalQuantity, string CountryOfOrigin, string CookNumber, string? Notes);
}
