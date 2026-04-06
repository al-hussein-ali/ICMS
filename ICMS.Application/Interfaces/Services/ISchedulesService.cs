using ICMS.Application.DTOs.Schedules;

namespace ICMS.Application.Interfaces.Services
{
    public interface ISchedulesService
    {
        Task<IEnumerable<ScheduleReadDto>> GetIndividualSchedulesAsync(int individualId, CancellationToken ct = default);
    }
}
