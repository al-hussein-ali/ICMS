using ICMS.Application.DTOs.Geography;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationReadDto>> GetGovernoratesAsync(CancellationToken ct = default);
        
        Task<IEnumerable<LocationReadDto>> GetDirectoratesByGovernorateAsync(int governorateId, CancellationToken ct = default);
        
        Task<IEnumerable<LocationReadDto>> GetNeighborhoodsByDirectorateAsync(int directorateId, CancellationToken ct = default);
        
        Task<IEnumerable<LocationReadDto>> GetSubNeighborhoodsByNeighborhoodAsync(int neighborhoodId, CancellationToken ct = default);
    }
}
