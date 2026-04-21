using ICMS.Domain.Entites.Clinical;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IVaccineRepository : IRepository<Vaccine, int>
    {
        new Task<Vaccine?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Vaccine>> SearchByNameAsync(string name, CancellationToken ct = default);
    }
}
