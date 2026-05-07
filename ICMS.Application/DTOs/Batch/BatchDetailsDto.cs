using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchDetailsDto(
        int Id, 
        string BatchName, 
        int DoseId, 
        string DoseName,
        string VaccineName,
        int UserId, 
        DateOnly CreationDate, 
        DateOnly ExpiryDate, 
        int TotalQuantity, 
        int RemainingQuantity,
        int ConsumedQuantity,
        string CountryOfOrigin, 
        string CookNumber, 
        string? Notes,
        string Status
    );
}
