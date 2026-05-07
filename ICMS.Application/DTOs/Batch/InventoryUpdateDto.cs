using System;

namespace ICMS.Application.DTOs.Batch
{
    public record InventoryUpdateDto(
        int Quantity, 
        string PermissionNumber, 
        string SourceOrDestination,
        DateTime? TransactionDate = null,
        string? Notes = null
    );
}
