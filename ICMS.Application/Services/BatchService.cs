using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace ICMS.Application.Services
{
    public class BatchService(IUnitOfWork unitOfWork, ICacheService cacheService) : IBatchService
    {

        public async Task<PagedResult<BatchReadDto>> GetAllAsync(BatchFilterDto filter,
            PaginationParams paginationParams, CancellationToken ct = default)
        {
            var pagedBatches = await unitOfWork.BatchRepository.GetAllAsync(filter, paginationParams, ct);

            var dtos = pagedBatches.Items.Select(b => b.ToReadDto()).ToList();

            return new PagedResult<BatchReadDto>(
                dtos,
                pagedBatches.TotalCount,
                pagedBatches.PageNumber,
                pagedBatches.PageSize);
        }

        public async Task<BatchDetailsDto?> GetByIdAsync(int batchId, CancellationToken ct = default)
        {
            string cacheKey = $"batch:{batchId}";
            if (cacheService.TryGet(cacheKey, out BatchDetailsDto? cached) && cached != null)
                return cached;

            var batch = await unitOfWork.BatchRepository.GetByIdWithDetailsAsync(batchId, ct);
            var dto = batch?.ToDetailsDto();

            if (dto != null)
                cacheService.Set(cacheKey, dto, TimeSpan.FromMinutes(2)); // Short TTL for inventory

            return dto;
        }

        public async Task<BatchReadDto> AddAsync(BatchCreateDto dto, int userId, CancellationToken ct = default)
        {
            var batch = dto.ToDomain(userId);
            await unitOfWork.BatchRepository.AddAsync(batch, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return batch.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int batchId, BatchUpdateDto dto, CancellationToken ct = default)
        {
            var batch = await unitOfWork.BatchRepository.GetByIdAsync(batchId, ct);
            if (batch == null) throw new NotFoundException("NotFound");

            batch.UpdateBatchInfo(dto.BatchName, dto.CountryOfOrigin, dto.CookNumber, dto.ExpiryDate, dto.Notes);

            var result = await unitOfWork.SaveChangesAsync(ct) > 0;
            if (result) cacheService.Remove($"batch:{batchId}");
            
            return result;
        }

        public async Task AddStockAsync(int batchId, InventoryUpdateDto dto, int userId, CancellationToken ct = default)
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var batch = await unitOfWork.BatchRepository.GetByIdAsync(batchId, ct);
                if (batch == null) throw new NotFoundException("NotFound");

                batch.AddInventory(dto.Quantity, dto.PermissionNumber, dto.SourceOrDestination, userId);
                await unitOfWork.SaveChangesAsync(ct);
                cacheService.Remove($"batch:{batchId}");
            });
        }

        public async Task RemoveStockAsync(int batchId, InventoryUpdateDto dto, int userId,
            CancellationToken ct = default)
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var batch = await unitOfWork.BatchRepository.GetByIdAsync(batchId, ct);
                if (batch == null) throw new NotFoundException("NotFound");


                batch.RemoveInventory(dto.Quantity, dto.PermissionNumber, dto.SourceOrDestination, userId);
                await unitOfWork.SaveChangesAsync(ct);
                cacheService.Remove($"batch:{batchId}");
            });
        }

        public async Task RemoveStockByDoseAsync(InventoryRemoveByDoseDto dto, int userId,
            CancellationToken ct = default)
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var batches = unitOfWork.BatchRepository.GetQueryable(track: true)
                    .Where(b => b.DoseId == dto.DoseId && b.TotalQuantity > 0)
                    .OrderBy(b => b.ExpiryDate)
                    .ToList(); // ToList because we need to iterate and modify

                int remainingToSubtract = dto.Quantity;
                int totalAvailable = batches.Sum(b => b.TotalQuantity);

                if (totalAvailable < dto.Quantity)
                    throw new DomainException("InsufficientStock");

                string permissionNumber = string.IsNullOrWhiteSpace(dto.PermissionNumber) ? $"IMM-SUB-{DateTime.Now:yyyyMMddHHmmss}" : dto.PermissionNumber;
                string destination = string.IsNullOrWhiteSpace(dto.Destination) ? "The Center" : dto.Destination;

                foreach (var batch in batches)
                {
                    if (remainingToSubtract <= 0) break;

                    int toSubtractFromThisBatch = Math.Min(batch.TotalQuantity, remainingToSubtract);
                    batch.RemoveInventory(toSubtractFromThisBatch, permissionNumber, destination, userId);
                    remainingToSubtract -= toSubtractFromThisBatch;
                    cacheService.Remove($"batch:{batch.Id}");
                }

                await unitOfWork.SaveChangesAsync(ct);
            });
        }

        public Task<PagedResult<TransactionReadDto>> GetTransactionsAsync(int batchId,
            TransactionFilterDto filter, PaginationParams paginationParams, CancellationToken ct = default)
        {
            var query = unitOfWork.TransactionRepository.GetQueryable()
                .Where(t => t.BatchId == batchId);

            if (filter.TransactionType.HasValue)
            {
                query = query.Where(t => t.TransactionType == filter.TransactionType.Value);
            }

            query = query.OrderByDescending(t => t.TransactionDate);

            var totalCount = query.Count();
            var items = query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToList();

            var dtos = items.Select(t => t.ToReadDto()).ToList();

            return Task.FromResult(new PagedResult<TransactionReadDto>(
                dtos,
                totalCount,
                paginationParams.PageNumber,
                paginationParams.PageSize));
        }

        public async Task<bool> DeactivateAsync(int batchId, CancellationToken ct = default)
        {
            var batch = await unitOfWork.BatchRepository.GetByIdAsync(batchId, ct);
            if (batch == null) throw new NotFoundException("NotFound");

            batch.Deactivate();
            var result = await unitOfWork.SaveChangesAsync(ct) > 0;
            if (result) cacheService.Remove($"batch:{batchId}");
            return result;
        }
    }
}
