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
                .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Pending && s.ScheduledDate < cutoffDate)
                .ToListAsync(ct);
        }

        public async Task<List<MissedScheduleReadDto>> GetMissedSchedulesDetailedAsync(DateOnly fromDate, DateOnly toDate, CancellationToken ct = default)
        {
            return await _context.VaccinationSchedules
                .AsNoTracking()
                .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed &&
                            s.ScheduledDate >= fromDate &&
                            s.ScheduledDate <= toDate)
                .Select(s => new MissedScheduleReadDto(
                    s.VaccinatedIndividualId,
                    s.VaccinatedIndividual.Person.FirstName,
                    s.VaccinatedIndividual.Person.LastName,
                    s.Dose.DoseName,
                    s.ScheduledDate,
                    s.VaccinatedIndividual.Directorate != null ? s.VaccinatedIndividual.Directorate.Name : "N/A"
                ))
                .ToListAsync(ct);
        }
    }
}
