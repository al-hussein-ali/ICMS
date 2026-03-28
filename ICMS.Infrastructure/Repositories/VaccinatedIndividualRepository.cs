using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace ICMS.Infrastructure.Repositories
{
    public class VaccinatedIndividualRepository : Repository<VaccinatedIndividual, int>, IVaccinatedIndividualRepository
    {
        public VaccinatedIndividualRepository(AppDbContext context) : base(context)
        {
        }

        public new async Task<VaccinatedIndividual?> GetByIdAsync(int id,CancellationToken ct = default)
        {
            return await _dbSet.Include(vi => vi.Person).Include(vi => vi.User)
                .Where(vi => !vi.Person.IsDeleted).FirstOrDefaultAsync(vi => vi.Id == id, ct);
        }

        public async Task<VaccinatedIndividual?> GetDetailsById(int id, CancellationToken ct = default)
        {
            return await _dbSet.Include(vi => vi.Person).Include(vi => vi.User).Include(vi => vi.ImmunizationRecords)
                .Where(vi => !vi.Person.IsDeleted).FirstOrDefaultAsync(vi => vi.Id == id, ct);
        }

        public async Task<VaccinatedIndividual?> GetDetailsByCardNumber(string cardNumber, CancellationToken ct = default)
        {
            return await _dbSet.Include(vi => vi.Person).Include(vi => vi.User).Include(vi => vi.ImmunizationRecords)
                .Where(vi => !vi.Person.IsDeleted).FirstOrDefaultAsync(vi => vi.CardNumber == cardNumber,ct);
        }

        public async Task BulkInsertAsync(List<VaccinatedIndividual> vaccinatedIndividuals, CancellationToken ct = default)
        {
            await _context.BulkInsertAsync(vaccinatedIndividuals,new BulkConfig { SetOutputIdentity = true,IncludeGraph = true},cancellationToken:ct);
        }

    }
}
