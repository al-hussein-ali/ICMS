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
    public class DirectorateRepository : Repository<Directorate, int>, IDirectorateRepository
    {
        public DirectorateRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Directorate>> GetByGovernorateIdAsync(int governorateId, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(d => d.GovernorateId == governorateId)
                .ToListAsync(ct);
        }
    }
}
