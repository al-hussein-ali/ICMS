namespace ICMS.Application.DTOs.Batch
{
    public record InventoryRemoveByDoseDto(int DoseId, int Quantity, string? PermissionNumber = null, string? Destination = null);
}
