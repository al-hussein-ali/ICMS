using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchReadDto(
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
        string CookNumber,
        string CountryOfOrigin,
        string Status
    );
}
