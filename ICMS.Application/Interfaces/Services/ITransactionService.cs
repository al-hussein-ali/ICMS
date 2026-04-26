using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<IReadOnlyList<TransactionReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<TransactionReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<TransactionReadDto> AddAsync(TransactionCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, TransactionCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
