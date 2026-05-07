using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Clinical
{
    public class DoseRepository : Repository<Dose,int>,IDoseRepository
    {
        public DoseRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<IReadOnlyList<Dose>> GetAllAsync(int? vaccineId, CancellationToken ct = default)
        {
            var query = _dbSet.Include(x => x.Vaccine).AsNoTracking();

            if (vaccineId != null)
                query = query.Where(x => x.VaccineId == vaccineId);

            return await query.ToListAsync(ct);
        }

        public async Task<Dose?> GetByAsync(Expression<Func<Dose, bool>> expression, CancellationToken ct = default)
        {
            return await _dbSet.Include(x => x.Vaccine).FirstOrDefaultAsync(expression, ct);
        }

        public async Task<Dose?> GetNextDoseInSequenceAsync(int vaccineId, byte currentDoseOrder, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(x => x.Vaccine)
                .Where(d => d.VaccineId == vaccineId && d.DoseOrder > currentDoseOrder)
                .OrderBy(d => d.DoseOrder)
                .FirstOrDefaultAsync(ct);
        }
    }
}
