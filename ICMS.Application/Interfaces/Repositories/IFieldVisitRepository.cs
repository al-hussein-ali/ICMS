using ICMS.Domain.Entites.Visits;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IFieldVisitRepository : IRepository<FieldVisit, int>
    {
        Task<FieldVisit?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
        Task<ICMS.Domain.ValueObjects.PagedResult<FieldVisit>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, bool? onlyUncompleted = null, CancellationToken ct = default);
    }
}
