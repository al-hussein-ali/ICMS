using ICMS.Domain.Enums;
using System;

namespace ICMS.Application.DTOs.Transaction
{
    public record TransactionCreateDto(int BatchId, TransactionType TransactionType, DateTime TransactionDate, int Quantity, string PermissionNumber, string SourceOrDestination, string? Notes);
}
