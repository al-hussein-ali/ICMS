using ICMS.Domain.Entites.Clinical;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.DTOs.Schedules;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IVaccinationScheduleRepository : IRepository<VaccinationSchedule, int>
    {
        Task BulkInsertAsync(IEnumerable<VaccinationSchedule> schedules, CancellationToken ct = default);
        Task<List<VaccinationSchedule>> GetOverduePendingSchedulesAsync(DateOnly cutoffDate, CancellationToken ct = default);
        Task<List<MissedScheduleReadDto>> GetMissedSchedulesDetailedAsync(DateOnly fromDate, DateOnly toDate, int? subNeighborhoodId = null, int? workerId = null, CancellationToken ct = default);
    }
}
