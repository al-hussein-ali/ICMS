using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Visits;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Visits
{
    public class FieldVisitRepository : Repository<FieldVisit, int>, IFieldVisitRepository
    {
        public FieldVisitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<FieldVisit?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(fv => fv.ImmunizationRecords)
                .Include(fv => fv.SubNeighborhood)
                    .ThenInclude(sn => sn.Neighborhood)
                        .ThenInclude(n => n.Directorate)
                            .ThenInclude(d => d.Governorate)
                .Include(fv => fv.FieldVisitIndividuals)
                    .ThenInclude(fvi => fvi.VaccinatedIndividual)
                        .ThenInclude(vi => vi.Person)
                .Include(fv => fv.FieldVisitIndividuals)
                    .ThenInclude(fvi => fvi.VaccinatedIndividual)
                        .ThenInclude(vi => vi.Schedules)
                            .ThenInclude(s => s.Dose)
                .Include(fv => fv.FieldVisitWorkers)
                    .ThenInclude(fvw => fvw.User)
                        .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(fv => fv.Id == id, ct);
        }

        public async Task<ICMS.Domain.ValueObjects.PagedResult<FieldVisit>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, bool? onlyUncompleted = null, int? workerId = null, CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (onlyUncompleted == true)
            {
                query = query.Where(fv => !fv.IsCompleted);
            }

            if (workerId.HasValue)
            {
                query = query.Where(fv => fv.FieldVisitWorkers.Any(fvw => fvw.UserId == workerId.Value));
            }

            query = query
                .Include(fv => fv.SubNeighborhood)
                    .ThenInclude(sn => sn.Neighborhood)
                        .ThenInclude(n => n.Directorate)
                            .ThenInclude(d => d.Governorate);

            var totalCount = await query.CountAsync(ct);
            var itemsWithCounts = await query
                .OrderByDescending(fv => fv.VisitDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fv => new
                {
                    FieldVisit = fv,
                    ExplicitCount = fv.FieldVisitIndividuals.Count(),
                    FallbackCount = _context.VaccinationSchedules
                        .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed &&
                                    s.ScheduledDate >= fv.FromDate &&
                                    s.ScheduledDate <= fv.ToDate &&
                                    s.VaccinatedIndividual.SubNeighborhoodId == fv.SubNeighborhoodId &&
                                    s.VaccinatedIndividual.Person.DateOfBirth.AddMonths(s.Dose.Vaccine.MaxEligibleAgeInMonths) >= fv.ToDate)
                        .Select(s => s.VaccinatedIndividualId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync(ct);

            var items = itemsWithCounts.Select(x =>
            {
                x.FieldVisit.TargetedCount = x.ExplicitCount > 0 ? x.ExplicitCount : x.FallbackCount;
                return x.FieldVisit;
            }).ToList();

            return new ICMS.Domain.ValueObjects.PagedResult<FieldVisit>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<FieldVisit>> GetExpiredUncompletedVisitsAsync(DateOnly thresholdDate, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(fv => !fv.IsCompleted && fv.ToDate <= thresholdDate)
                .ToListAsync(ct);
        }

        public async Task<List<FieldVisit>> GetUpcomingVisitsForRemindersAsync(DateOnly date, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(fv => fv.FieldVisitIndividuals)
                .Where(fv => fv.VisitDate == date && !fv.ReminderSent && !fv.IsCompleted)
                .ToListAsync(ct);
        }

        public async Task<object> GetDiagnosticDbAsync(CancellationToken ct = default)
        {
            var visits = await _dbSet.AsNoTracking()
                .Select(fv => new {
                    fv.Id,
                    fv.CampaignName,
                    fv.IsCompleted,
                    IndividualsCount = fv.FieldVisitIndividuals.Count,
                    WorkersCount = fv.FieldVisitWorkers.Count
                })
                .ToListAsync(ct);

            var totalIndividuals = await _dbSet.AsNoTracking()
                .SelectMany(fv => fv.FieldVisitIndividuals).CountAsync(ct);
                
            var totalWorkers = await _dbSet.AsNoTracking()
                .SelectMany(fv => fv.FieldVisitWorkers).CountAsync(ct);

            var individuals = await _context.VaccinatedIndividuals.AsNoTracking()
                .Select(vi => new { vi.Id, vi.CardNumber })
                .Take(5)
                .ToListAsync(ct);

            var subNeighborhoods = await _context.SubNeighborhoods.AsNoTracking()
                .Select(sn => new { sn.Id, sn.Name })
                .Take(5)
                .ToListAsync(ct);

            var workers = await _context.Users.AsNoTracking()
                .Select(u => new { u.Id, u.UserName })
                .Take(5)
                .ToListAsync(ct);

            return new {
                TotalVisits = visits.Count,
                TotalIndividualsInRelations = totalIndividuals,
                TotalWorkersInRelations = totalWorkers,
                Visits = visits,
                SampleIndividuals = individuals,
                SampleSubNeighborhoods = subNeighborhoods,
                SampleWorkers = workers
            };
        }
    }
}
