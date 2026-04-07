using ICMS.Application.DTOs.Schedules;

namespace ICMS.Application.Interfaces.Services
{
    public interface ISchedulesService
    {
        Task<IEnumerable<ScheduleReadDto>> GetIndividualSchedulesAsync(int individualId, CancellationToken ct = default);
        Task<IEnumerable<MissedScheduleReadDto>> GetMissedSchedulesDetailedAsync(MissedScheduleQueryDto query, CancellationToken ct = default);
    }
}
