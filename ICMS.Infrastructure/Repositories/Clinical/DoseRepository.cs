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
            if (vaccineId == null)
                return await _dbSet.AsNoTracking().ToListAsync();

            return await _dbSet.Where(x => x.VaccineId == vaccineId).AsNoTracking().ToListAsync();
        }

        public async Task<Dose?> GetByAsync(Expression<Func<Dose, bool>> expression, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(expression,ct);
        }

        public async Task<Dose?> GetNextDoseInSequenceAsync(int vaccineId, byte currentDoseOrder, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(d => d.VaccineId == vaccineId && d.DoseOrder > currentDoseOrder)
                .OrderBy(d => d.DoseOrder)
                .FirstOrDefaultAsync(ct);
        }
    }
}
