using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchReadDto(int Id, string BatchName, int DoseId, int UserId, DateOnly CreationDate, DateOnly ExpiryDate, int TotalQuantity);
}
