using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Identity;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using ICMS.Domain.ValueObjects;

namespace ICMS.Infrastructure.Repositories.Identity
{
    public class VaccinatedIndividualRepository(AppDbContext context) : Repository<VaccinatedIndividual, int>(context), IVaccinatedIndividualRepository
    {
        public async Task<VaccinatedIndividual?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(vi => vi.Person)
                .Include(vi => vi.User)
                .Include(vi => vi.Directorate)
                    .ThenInclude(d => d.Governorate)
                .Include(vi => vi.Neighborhood)
                .Where(vi => !vi.Person.IsDeleted)
                .FirstOrDefaultAsync(vi => vi.Id == id, ct);
        }

        public async Task<VaccinatedIndividual?> GetDetailsById(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(vi => vi.Person)
                .Include(vi => vi.User)
                .Include(vi => vi.Directorate)
                    .ThenInclude(d => d.Governorate)
                .Include(vi => vi.Neighborhood)
                .Include(vi => vi.ImmunizationRecords)
                .Where(vi => !vi.Person.IsDeleted)
                .FirstOrDefaultAsync(vi => vi.Id == id, ct);
        }

        public async Task<VaccinatedIndividual?> GetDetailsByCardNumber(string cardNumber, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(vi => vi.Person)
                .Include(vi => vi.User)
                .Include(vi => vi.Directorate)
                    .ThenInclude(d => d.Governorate)
                .Include(vi => vi.Neighborhood)
                .Include(vi => vi.ImmunizationRecords)
                .Where(vi => !vi.Person.IsDeleted)
                .FirstOrDefaultAsync(vi => vi.CardNumber == cardNumber, ct);
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

        public IQueryable<VaccinatedIndividual> GetQueryableWithDetails(CancellationToken ct = default)
        {
            return _dbSet
                .Include(vi => vi.Person)
                .Include(vi => vi.Directorate)
                    .ThenInclude(d => d.Governorate)
                .Include(vi => vi.Neighborhood);
        }

        public async Task<PagedResult<VaccinatedIndividual>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = GetQueryableWithDetails(ct);
            var totalCount = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync(ct);

            return new PagedResult<VaccinatedIndividual>(items, totalCount, pageNumber, pageSize);
        }
    }
}
