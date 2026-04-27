using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Visits;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Repositories.Visits
{
    public class FieldVisitRepository : Repository<FieldVisit, int>, IFieldVisitRepository
    {
        public FieldVisitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<FieldVisit?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(fv => fv.ImmunizationRecords)
                .Include(fv => fv.SubNeighborhood)
                    .ThenInclude(sn => sn.Neighborhood)
                        .ThenInclude(n => n.Directorate)
                            .ThenInclude(d => d.Governorate)
                .FirstOrDefaultAsync(fv => fv.Id == id, ct);
        }

        public async Task<ICMS.Domain.ValueObjects.PagedResult<FieldVisit>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = _dbSet
                .Include(fv => fv.SubNeighborhood)
                    .ThenInclude(sn => sn.Neighborhood)
                        .ThenInclude(n => n.Directorate)
                            .ThenInclude(d => d.Governorate);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(fv => fv.VisitDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new ICMS.Domain.ValueObjects.PagedResult<FieldVisit>(items, totalCount, pageNumber, pageSize);
        }
    }
}
