using ICMS.Application.DTOs.Transaction;
using ICMS.Domain.Entites.Audit;

namespace ICMS.Application.Extensions
{
    public static class TransactionExtensions
    {
        public static TransactionReadDto ToReadDto(this Transaction t)
            => new(
                t.Id, 
                t.BatchId, 
                t.Batch?.CookNumber ?? "N/A",
                LocalizationHelper.GetLocalizedValue(t.Batch?.Dose?.Vaccine?.VaccineName) ?? "N/A",
                t.TransactionType, 
                t.TransactionDate, 
                t.Quantity,
                t.PermissionNumber,
                t.SourceOrDestination,
                t.Notes,
                t.User?.UserName ?? "System"
            );

        public static TransactionDetailsDto ToDetailsDto(this Transaction t)
            => new(
                t.Id, 
                t.BatchId, 
                t.TransactionType, 
                t.TransactionDate, 
                t.Quantity, 
                t.PermissionNumber, 
                t.SourceOrDestination,
                t.Notes
            );

        public static Transaction ToDomain(this TransactionCreateDto dto, int userId)
            => Transaction.Create(
                dto.BatchId, 
                dto.TransactionType, 
                dto.TransactionDate, 
                dto.Quantity, 
                dto.PermissionNumber, 
                dto.SourceOrDestination, 
                userId,
                dto.Notes
            );
    }
}
