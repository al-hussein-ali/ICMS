using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories
{
    public class VaccinatedIndividualRepository : Repository<VaccinatedIndividual,int>, IVaccinatedIndividualRepository
    {
        public VaccinatedIndividualRepository(AppDbContext context) : base(context)
        {
        }

        public new async Task<VaccinatedIndividual?> GetByIdAsync(int id,CancellationToken ct = default)
        {
            return await _dbSet.Include(vi => vi.Person).Include(vi => vi.User).Where(vi => !vi.Person.IsDeleted).FirstOrDefaultAsync(vi => vi.Id == id, ct);
        }

    }
}
