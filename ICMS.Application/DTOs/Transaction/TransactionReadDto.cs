using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Transaction
{
    public record TransactionReadDto(
        Guid Id, 
        int BatchId, 
        string CookNumber,
        string VaccineName,
        TransactionType TransactionType, 
        DateTime TransactionDate, 
        int Quantity,
        string PermissionNumber,
        string SourceOrDestination,
        string? Notes,
        string UserName
    );
}
