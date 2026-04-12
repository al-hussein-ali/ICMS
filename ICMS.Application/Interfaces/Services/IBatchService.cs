using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IBatchService
    {
        Task<PagedResult<BatchReadDto>> GetAllAsync(BatchFilterDto filter, PaginationParams paginationParams, CancellationToken ct = default);
        Task<BatchDetailsDto?> GetByIdAsync(int batchId, CancellationToken ct = default);
        Task<BatchReadDto> AddAsync(BatchCreateDto dto, int userId, CancellationToken ct = default);
        Task<bool> UpdateAsync(int batchId, BatchCreateDto dto, CancellationToken ct = default);
        Task AddStockAsync(int batchId, InventoryUpdateDto dto, int userId, CancellationToken ct = default);
        Task RemoveStockAsync(int batchId, InventoryUpdateDto dto, int userId, CancellationToken ct = default);
        Task<PagedResult<TransactionReadDto>> GetTransactionsAsync(int batchId, PaginationParams paginationParams, CancellationToken ct = default);
    }
}
