using ICMS.Domain.Entites.Visits;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IFieldVisitRepository : IRepository<FieldVisit, int>
    {
        Task<FieldVisit?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
    }
}
