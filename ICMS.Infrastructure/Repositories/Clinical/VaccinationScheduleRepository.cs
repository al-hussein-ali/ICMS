using EFCore.BulkExtensions;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Clinical;
using ICMS.Infrastructure.Persistence.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    }
}
