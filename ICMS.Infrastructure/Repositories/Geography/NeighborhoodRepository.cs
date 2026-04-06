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
    public class NeighborhoodRepository : Repository<Neighborhood, int>, INeighborhoodRepository
    {
        public NeighborhoodRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Neighborhood>> GetByDirectorateIdAsync(int directorateId, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(n => n.DirectorateId == directorateId)
                .ToListAsync(ct);
        }
    }
}
