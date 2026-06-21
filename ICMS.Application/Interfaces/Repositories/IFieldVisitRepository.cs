using ICMS.Domain.Entites.Visits;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IFieldVisitRepository : IRepository<FieldVisit, int>
    {
        Task<FieldVisit?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
        Task<ICMS.Domain.ValueObjects.PagedResult<FieldVisit>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, bool? onlyUncompleted = null, int? workerId = null, CancellationToken ct = default);
        Task<List<FieldVisit>> GetExpiredUncompletedVisitsAsync(DateOnly thresholdDate, CancellationToken ct = default);
        Task<List<FieldVisit>> GetUpcomingVisitsForRemindersAsync(DateOnly date, CancellationToken ct = default);
        Task<object> GetDiagnosticDbAsync(CancellationToken ct = default);
    }
}
