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
using EFCore.BulkExtensions;

namespace ICMS.Infrastructure.Repositories.Identity
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

        public async Task<VaccinatedIndividual?> GetByPersonIdAsync(int personId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(vi => vi.Person)
                .Where(vi => !vi.Person.IsDeleted)
                .FirstOrDefaultAsync(vi => vi.PersonId == personId, ct);
        }

        public async Task BulkInsertAsync(List<VaccinatedIndividual> vaccinatedIndividuals, CancellationToken ct = default)
        {
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                await _dbSet.AddRangeAsync(vaccinatedIndividuals, ct);
                // Note: Identity and Graph handling are built-in for standard EF AddRange in InMemory
            }
            else
            {
                await _context.BulkInsertAsync(vaccinatedIndividuals, new BulkConfig { SetOutputIdentity = true, IncludeGraph = true }, cancellationToken: ct);
            }
        }

        public async Task<List<VaccinatedIndividual>> GetByIdsWithImmunizationRecordsAsync(List<int> ids, CancellationToken ct = default)
        {
            return await _dbSet.Include(vi => vi.ImmunizationRecords)
                .Where(vi => ids.Contains(vi.Id))
                .ToListAsync(ct);
        }

        public async Task<VaccinatedIndividual?> GetIndividualWithSchedulesAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(vi => vi.Person)
                .Include(vi => vi.Schedules)
                    .ThenInclude(s => s.Dose)
                        .ThenInclude(d => d.Vaccine)
                .Include(vi => vi.ImmunizationRecords)
                .Where(vi => !vi.Person.IsDeleted)
                .FirstOrDefaultAsync(vi => vi.Id == id, ct);
        }



    }
}
