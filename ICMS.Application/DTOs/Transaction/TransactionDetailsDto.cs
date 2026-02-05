using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Transaction
{
    public record TransactionDetailsDto(Guid Id, int BatchId, TransactionType TransactionType, DateTime TransactionDate, int Quantity, string PermissionNumber, string SourceorDestination);
}
