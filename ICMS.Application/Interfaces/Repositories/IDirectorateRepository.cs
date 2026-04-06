using ICMS.Domain.Entites.Geography;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IDirectorateRepository : IRepository<Directorate, int>
    {
        Task<IEnumerable<Directorate>> GetByGovernorateIdAsync(int governorateId, CancellationToken ct = default);
    }
}
