using ICMS.Application.DTOs.DoseReport;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class DoseReportService(IUnitOfWork unitOfWork) : IDoseReportService
    {
        public async Task<PagedResult<DoseReportReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var query = unitOfWork.DoseReportRepository.GetQueryable()
                .Select(r => r.ToReadDto());
            
            return query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<DoseReportReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var report = await unitOfWork.DoseReportRepository.GetByIdAsync(id, ct);
            return report?.ToReadDto();
        }

        public async Task<DoseReportReadDto> AddAsync(DoseReportCreateDto dto, int userId, CancellationToken ct = default)
        {
            return await unitOfWork.ExecuteInTransactionAsync<DoseReportReadDto>(async () =>
            {
                var batch = await unitOfWork.BatchRepository.GetByIdAsync(dto.BatchId, ct);
                if (batch == null) throw new NotFoundException("NotFound");

                // Automatically zero out stock for reported batches to prevent further use
                if (batch.TotalQuantity > 0)
                {
                    batch.RemoveInventory(batch.TotalQuantity, "REPORTED", "DISCARDED", userId);
                    
                    // Explicitly add the new transaction to the context to ensure it is saved
                    var lastTransaction = batch.Transactions.Last();
                    await unitOfWork.TransactionRepository.AddAsync(lastTransaction, ct);
                }

                var report = dto.ToDomain(userId);

                await unitOfWork.DoseReportRepository.AddAsync(report, ct);
                await unitOfWork.SaveChangesAsync(ct);

                return report.ToReadDto();
            });
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var report = await unitOfWork.DoseReportRepository.GetByIdAsync(id, ct);
            if (report == null) return false;

            await unitOfWork.DoseReportRepository.DeleteAsync(report, ct);
            return await unitOfWork.SaveChangesAsync(ct) > 0;
        }
    }
}
