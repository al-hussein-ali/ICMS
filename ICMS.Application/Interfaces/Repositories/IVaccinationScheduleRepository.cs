using ICMS.Domain.Entites.Clinical;
using ICMS.Application.Interfaces.Repositories;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IVaccinationScheduleRepository : IRepository<VaccinationSchedule, int>
    {
        Task BulkInsertAsync(IEnumerable<VaccinationSchedule> schedules, CancellationToken ct = default);
    }
}
