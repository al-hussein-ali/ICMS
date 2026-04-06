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
    public class PregnancyDetailsRepository : Repository<PregnancyDetails,int>,IPregnancyDetailsRepository
    {
        public PregnancyDetailsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PregnancyDetails?> GetPregnancyWithDetailsAsync(int pregnancyId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(pd => pd.PregnantWoman)
                .Include(pd => pd.VisitDetails)
                .Include(pd => pd.Newborns)
                .Include(pd => pd.PreviousPregnancyComplications)
                .Include(pd => pd.PreviousPostpartumComplications)
                .Include(pd => pd.PreviousPregnancyDeliveryComplications)
                .FirstOrDefaultAsync(pd => pd.Id == pregnancyId, ct);
        }
    }
}
