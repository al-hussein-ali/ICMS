using EFCore.BulkExtensions;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Clinical;
using ICMS.Application.DTOs.Schedules;
using ICMS.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Repositories.Clinical
{
    public class VaccinationScheduleRepository : Repository<VaccinationSchedule, int>, IVaccinationScheduleRepository
    {
        public VaccinationScheduleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task BulkInsertAsync(IEnumerable<VaccinationSchedule> schedules, CancellationToken ct = default)
        {
            await _context.BulkInsertAsync(new List<VaccinationSchedule>(schedules), cancellationToken: ct);
        }

        public async Task<List<VaccinationSchedule>> GetOverduePendingSchedulesAsync(DateOnly cutoffDate, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(s => s.Dose)
                .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Pending && 
                            s.ScheduledDate < cutoffDate &&
                            s.Dose.IsPrimary)
                .ToListAsync(ct);
        }

        public async Task<List<MissedScheduleReadDto>> GetMissedSchedulesDetailedAsync(DateOnly fromDate, DateOnly toDate, int? subNeighborhoodId = null, int? workerId = null, CancellationToken ct = default)
        {
            var query = _context.VaccinationSchedules
                .AsNoTracking()
                .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed &&
                            s.ScheduledDate >= fromDate &&
                            s.ScheduledDate <= toDate &&
                            s.VaccinatedIndividual.Person.DateOfBirth.AddMonths(s.Dose.Vaccine.MaxEligibleAgeInMonths) >= toDate);

            if (subNeighborhoodId.HasValue)
            {
                query = query.Where(s => s.VaccinatedIndividual.SubNeighborhoodId == subNeighborhoodId.Value);
            }

            if (workerId.HasValue)
            {
                query = query.Where(s => _context.FieldVisitIndividuals
                    .Any(fvi => fvi.VaccinatedIndividualId == s.VaccinatedIndividualId && 
                                fvi.AssignedWorkerId == workerId.Value &&
                                !fvi.FieldVisit.IsCompleted));
            }

            return await query.Select(s => new MissedScheduleReadDto(
                    s.VaccinatedIndividualId,
                    s.VaccinatedIndividual.Person.FirstName,
                    s.VaccinatedIndividual.Person.LastName,
                    s.VaccinatedIndividual.Person.PhoneNumber,
                    s.VaccinatedIndividual.CardNumber,
                    s.DoseId,
                    s.Dose.DoseName
                ))
                .ToListAsync(ct);
        }
    }
}
