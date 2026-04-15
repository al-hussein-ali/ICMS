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
                .FirstOrDefaultAsync(fv => fv.Id == id, ct);
        }
    }
}
