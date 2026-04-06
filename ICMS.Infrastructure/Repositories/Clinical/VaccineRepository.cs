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
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Clinical
{
    public class VaccineRepository : Repository<Vaccine,int>, IVaccineRepository
    {
        public VaccineRepository(AppDbContext context) : base(context)
        {
        }

        public new async Task<Vaccine?> GetByIdAsync(int id,CancellationToken ct =default)
        {
            return await _dbSet.Include(v => v.Doses).FirstOrDefaultAsync(v => v.Id == id, ct); 
        }

    }
}
