using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BatchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<BatchReadDto>> GetAllAsync(BatchFilterDto filter, PaginationParams paginationParams, CancellationToken ct = default)
        {
            var pagedBatches = await _unitOfWork.BatchRepository.GetAllAsync(filter, paginationParams, ct);
            
            var dtos = pagedBatches.Items.Select(b => b.ToReadDto()).ToList();
            
            return new PagedResult<BatchReadDto>(
                dtos, 
                pagedBatches.TotalCount, 
                pagedBatches.PageNumber, 
                pagedBatches.PageSize);
        }

        public async Task<BatchDetailsDto?> GetByIdAsync(int batchId, CancellationToken ct = default)
        {
            var batch = await _unitOfWork.BatchRepository.GetByIdWithDetailsAsync(batchId, ct);
            return batch?.ToDetailsDto();
        }

        public async Task<BatchReadDto> AddAsync(BatchCreateDto dto, int userId, CancellationToken ct = default)
        {
            var batch = dto.ToDomain(userId);
            await _unitOfWork.BatchRepository.AddAsync(batch, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return batch.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int batchId, BatchCreateDto dto, CancellationToken ct = default)
        {
            // Direct update of Batch metadata is restricted as per domain rules.
            throw new DomainException("Direct update of Batch metadata is restricted. Please create a new batch if identifiers change.");
        }

        public async Task AddStockAsync(int batchId, InventoryUpdateDto dto, int userId, CancellationToken ct = default)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var batch = await _unitOfWork.BatchRepository.GetByIdAsync(batchId, ct);
                if (batch == null) throw new DomainException($"Batch {batchId} not found");

                batch.AddInventory(dto.Quantity, dto.PermissionNumber, dto.SourceOrDestination, userId);
                await _unitOfWork.SaveChangesAsync(ct);
            });
        }

        public async Task RemoveStockAsync(int batchId, InventoryUpdateDto dto, int userId, CancellationToken ct = default)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var batch = await _unitOfWork.BatchRepository.GetByIdAsync(batchId, ct);
                if (batch == null) throw new DomainException($"Batch {batchId} not found");

                batch.RemoveInventory(dto.Quantity, dto.PermissionNumber, dto.SourceOrDestination, userId);
                await _unitOfWork.SaveChangesAsync(ct);
            });
        }

        public async Task RemoveStockByDoseAsync(InventoryRemoveByDoseDto dto, int userId, CancellationToken ct = default)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var batches = _unitOfWork.BatchRepository.GetQueryable(track: true)
                    .Where(b => b.DoseId == dto.DoseId && b.TotalQuantity > 0)
                    .OrderBy(b => b.ExpiryDate)
                    .ToList(); // ToList because we need to iterate and modify

                int remainingToSubtract = dto.Quantity;
                int totalAvailable = batches.Sum(b => b.TotalQuantity);

                if (totalAvailable < dto.Quantity)
                    throw new DomainException($"Insufficient total inventory for Dose {dto.DoseId}. Available: {totalAvailable}, Requested: {dto.Quantity}");

                foreach (var batch in batches)
                {
                    if (remainingToSubtract <= 0) break;

                    int toSubtractFromThisBatch = Math.Min(batch.TotalQuantity, remainingToSubtract);
                    batch.RemoveInventory(toSubtractFromThisBatch, dto.PermissionNumber, dto.Destination, userId);
                    remainingToSubtract -= toSubtractFromThisBatch;
                }

                await _unitOfWork.SaveChangesAsync(ct);
            });
        }

        public async Task<PagedResult<TransactionReadDto>> GetTransactionsAsync(int batchId, TransactionFilterDto filter, PaginationParams paginationParams, CancellationToken ct = default)
        {
            var query = _unitOfWork.TransactionRepository.GetQueryable()
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
            
            return new PagedResult<TransactionReadDto>(
                dtos,
                totalCount,
                paginationParams.PageNumber,
                paginationParams.PageSize);
        }
    }
}
