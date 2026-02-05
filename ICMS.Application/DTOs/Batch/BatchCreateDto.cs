using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchCreateDto(int DoseId, int UserId, DateOnly ExpiryDate, int TotalQuantity, string? Notes);
}
