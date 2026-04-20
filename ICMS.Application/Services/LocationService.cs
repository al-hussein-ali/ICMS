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
        private readonly ICacheService _cacheService;

        public LocationService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<LocationReadDto>> GetGovernoratesAsync(CancellationToken ct = default)
        {
            const string cacheKey = "locations:governorates";
            if (_cacheService.TryGet(cacheKey, out IEnumerable<LocationReadDto>? cached) && cached != null)
                return cached;

            var governorates = await _unitOfWork.GovernorateRepository.GetAllAsync(false, ct);
            var dtos = governorates.Select(g => g.ToReadDto()).ToList();
            
            _cacheService.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return dtos;
        }

        public async Task<IEnumerable<LocationReadDto>> GetDirectoratesByGovernorateAsync(int governorateId, CancellationToken ct = default)
        {
            string cacheKey = $"locations:directorates:{governorateId}";
            if (_cacheService.TryGet(cacheKey, out IEnumerable<LocationReadDto>? cached) && cached != null)
                return cached;

            var directorates = await _unitOfWork.DirectorateRepository.GetByGovernorateIdAsync(governorateId, ct);
            var dtos = directorates.Select(d => d.ToReadDto()).ToList();

            _cacheService.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return dtos;
        }

        public async Task<IEnumerable<LocationReadDto>> GetNeighborhoodsByDirectorateAsync(int directorateId, CancellationToken ct = default)
        {
            string cacheKey = $"locations:neighborhoods:{directorateId}";
            if (_cacheService.TryGet(cacheKey, out IEnumerable<LocationReadDto>? cached) && cached != null)
                return cached;

            var neighborhoods = await _unitOfWork.NeighborhoodRepository.GetByDirectorateIdAsync(directorateId, ct);
            var dtos = neighborhoods.Select(n => n.ToReadDto()).ToList();

            _cacheService.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return dtos;
        }

        public async Task<IEnumerable<LocationReadDto>> GetSubNeighborhoodsByNeighborhoodAsync(int neighborhoodId, CancellationToken ct = default)
        {
            string cacheKey = $"locations:subneighborhoods:{neighborhoodId}";
            if (_cacheService.TryGet(cacheKey, out IEnumerable<LocationReadDto>? cached) && cached != null)
                return cached;

            var subNeighborhoods = await _unitOfWork.SubNeighborhoodRepository.GetByNeighborhoodIdAsync(neighborhoodId, ct);
            var dtos = subNeighborhoods.Select(s => s.ToReadDto()).ToList();

            _cacheService.Set(cacheKey, dtos, TimeSpan.FromHours(1));
            return dtos;
        }
    }
}
