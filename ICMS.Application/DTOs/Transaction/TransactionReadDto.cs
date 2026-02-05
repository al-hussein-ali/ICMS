using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.Transaction
{
    public record TransactionReadDto(Guid Id, int BatchId, TransactionType TransactionType, DateTime TransactionDate, int Quantity);
}
