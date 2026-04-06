using ICMS.Application.DTOs.Geography;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LocationReadDto>> GetGovernoratesAsync(CancellationToken ct = default)
        {
            var governorates = await _unitOfWork.GovernorateRepository.GetAllAsync(false, ct);
            return governorates.Select(g => g.ToReadDto());
        }

        public async Task<IEnumerable<LocationReadDto>> GetDirectoratesByGovernorateAsync(int governorateId, CancellationToken ct = default)
        {
            var directorates = await _unitOfWork.DirectorateRepository.GetByGovernorateIdAsync(governorateId, ct);
            return directorates.Select(d => d.ToReadDto());
        }

        public async Task<IEnumerable<LocationReadDto>> GetNeighborhoodsByDirectorateAsync(int directorateId, CancellationToken ct = default)
        {
            var neighborhoods = await _unitOfWork.NeighborhoodRepository.GetByDirectorateIdAsync(directorateId, ct);
            return neighborhoods.Select(n => n.ToReadDto());
        }

        public async Task<IEnumerable<LocationReadDto>> GetSubNeighborhoodsByNeighborhoodAsync(int neighborhoodId, CancellationToken ct = default)
        {
            var subNeighborhoods = await _unitOfWork.SubNeighborhoodRepository.GetByNeighborhoodIdAsync(neighborhoodId, ct);
            return subNeighborhoods.Select(s => s.ToReadDto());
        }
    }
}
