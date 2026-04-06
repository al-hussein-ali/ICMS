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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Maternal
{
    public class PregnantWomanRepository : Repository<PregnantWoman,int>,IPregnantWomanRepository
    {
        public PregnantWomanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PregnantWoman?> GetByPersonIdWithDetailsAsync(int personId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(pw => pw.PregnancyDetails)
                .FirstOrDefaultAsync(pw => pw.PersonId == personId, ct);
        }

        public async Task<PregnantWoman?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(pw => pw.Person)
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.VisitDetails)
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.Newborns)
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.PreviousPregnancyComplications)
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.PreviousPostpartumComplications)
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.PreviousPregnancyDeliveryComplications)
                .FirstOrDefaultAsync(pw => pw.Id == id, ct);
        }
    }
}
