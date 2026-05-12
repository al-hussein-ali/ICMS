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

        public async Task<ICMS.Domain.ValueObjects.PagedResult<FieldVisit>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, bool? onlyUncompleted = null, CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (onlyUncompleted == true)
            {
                query = query.Where(fv => !fv.IsCompleted);
            }

            query = query
                .Include(fv => fv.SubNeighborhood)
                    .ThenInclude(sn => sn.Neighborhood)
                        .ThenInclude(n => n.Directorate)
                            .ThenInclude(d => d.Governorate);

            var totalCount = await query.CountAsync(ct);
            var itemsWithCounts = await query
                .OrderByDescending(fv => fv.VisitDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fv => new
                {
                    FieldVisit = fv,
                    TargetedCount = _context.VaccinationSchedules
                        .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed &&
                                   s.VaccinatedIndividual.SubNeighborhoodId == fv.SubNeighborhoodId &&
                                   s.ScheduledDate >= fv.FromDate &&
                                   s.ScheduledDate <= fv.ToDate)
                        .Select(s => s.VaccinatedIndividualId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync(ct);

            var items = itemsWithCounts.Select(x =>
            {
                x.FieldVisit.TargetedCount = x.TargetedCount;
                return x.FieldVisit;
            }).ToList();

            return new ICMS.Domain.ValueObjects.PagedResult<FieldVisit>(items, totalCount, pageNumber, pageSize);
        }
    }
}
