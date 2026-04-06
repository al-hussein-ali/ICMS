using ICMS.Application.DTOs.Transaction;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class TransactionExtensions
    {
        public static TransactionReadDto ToReadDto(this Transaction t)
            => new(t.Id, t.BatchId, t.TransactionType, t.TransactionDate, t.Quantity);

        public static TransactionDetailsDto ToDetailsDto(this Transaction t)
            => new(t.Id, t.BatchId, t.TransactionType, t.TransactionDate, t.Quantity, t.PermissionNumber, t.SourceorDestination);

        public static Transaction ToDomain(this TransactionCreateDto dto)
            => Transaction.Create(dto.BatchId,dto.TransactionType,dto.TransactionDate,dto.Quantity,dto.PermissionNumber,dto.SourceorDestination); // factory not implemented
    }
}
