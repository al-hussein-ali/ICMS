using ICMS.Domain.Entites.Geography;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface INeighborhoodRepository : IRepository<Neighborhood, int>
    {
        Task<IEnumerable<Neighborhood>> GetByDirectorateIdAsync(int directorateId, CancellationToken ct = default);
    }
}
