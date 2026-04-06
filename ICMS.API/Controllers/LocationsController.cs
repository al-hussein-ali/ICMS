using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("governorates")]
        public async Task<IActionResult> GetGovernoratesAsync(CancellationToken ct)
        {
            var result = await _locationService.GetGovernoratesAsync(ct);
            return Ok(result);
        }

        [HttpGet("directorates/{governorateId}")]
        public async Task<IActionResult> GetDirectoratesAsync(int governorateId, CancellationToken ct)
        {
            var result = await _locationService.GetDirectoratesByGovernorateAsync(governorateId, ct);
            return Ok(result);
        }

        [HttpGet("neighborhoods/{directorateId}")]
        public async Task<IActionResult> GetNeighborhoodsAsync(int directorateId, CancellationToken ct)
        {
            var result = await _locationService.GetNeighborhoodsByDirectorateAsync(directorateId, ct);
            return Ok(result);
        }

        [HttpGet("sub-neighborhoods/{neighborhoodId}")]
        public async Task<IActionResult> GetSubNeighborhoodsAsync(int neighborhoodId, CancellationToken ct)
        {
            var result = await _locationService.GetSubNeighborhoodsByNeighborhoodAsync(neighborhoodId, ct);
            return Ok(result);
        }
    }
}
