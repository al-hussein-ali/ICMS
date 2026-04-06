using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Geography;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Geography
{
    public class SubNeighborhoodRepository : Repository<SubNeighborhood, int>, ISubNeighborhoodRepository
    {
        public SubNeighborhoodRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SubNeighborhood>> GetByNeighborhoodIdAsync(int neighborhoodId, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(s => s.NeighborhoodId == neighborhoodId)
                .ToListAsync(ct);
        }
    }
}
