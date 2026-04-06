using ICMS.Domain.Entites.Geography;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface ISubNeighborhoodRepository : IRepository<SubNeighborhood, int>
    {
        Task<IEnumerable<SubNeighborhood>> GetByNeighborhoodIdAsync(int neighborhoodId, CancellationToken ct = default);
    }
}
