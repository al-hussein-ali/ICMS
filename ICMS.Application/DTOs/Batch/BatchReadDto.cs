using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchReadDto(int Id, int DoseId, int UserId, DateOnly ExpiryDate, int TotalQuantity);
}
