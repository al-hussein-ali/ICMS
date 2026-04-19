namespace ICMS.Application.DTOs.Batch
{
    public record InventoryRemoveByDoseDto(int DoseId, int Quantity, string PermissionNumber, string Destination);
}
