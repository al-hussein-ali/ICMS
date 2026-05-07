using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.ValueObjects;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Repositories.Clinical
{
    public class BatchRepository : Repository<Batch, int>, IBatchRepository
    {
        public BatchRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Batch>> GetAllAsync(BatchFilterDto filter, PaginationParams paginationParams,
            CancellationToken ct = default)
        {
            var query = _context.Batches
                .Include(b => b.Dose)
                    .ThenInclude(d => d.Vaccine)
                .AsNoTracking();

            if (!filter.IncludeInactive)
            {
                query = query.Where(b => b.IsActive);
            }

            if (filter.DoseId.HasValue)
            {
                query = query.Where(b => b.DoseId == filter.DoseId.Value);
            }

            if (filter.ExpiryDate.HasValue)
            {
                query = query.Where(b => b.ExpiryDate == filter.ExpiryDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CookNumber))
            {
                query = query.Where(b => b.CookNumber.Contains(filter.CookNumber));
            }

            if (!string.IsNullOrWhiteSpace(filter.VaccineName))
            {
                query = query.Where(b => b.Dose!.Vaccine.VaccineName.Contains(filter.VaccineName));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Batch>(items, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<Batch?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default, bool track = false)
        {
            var query = _context.Batches.AsQueryable();
            if (!track) query = query.AsNoTracking();

            return await query
                .Include(b => b.Dose)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id, ct);
        }
    }
}
