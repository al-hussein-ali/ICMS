using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/locations")]
    [ApiController]
    [Authorize(Roles = Roles.StaffRoles)]
    public class LocationsController(ILocationService locationService) : ControllerBase
    {

        [HttpGet("governorates")]
        public async Task<IActionResult> GetGovernoratesAsync(CancellationToken ct)
        {
            var result = await locationService.GetGovernoratesAsync(ct);
            return Ok(result);
        }

        [HttpGet("directorates/{governorateId}")]
        public async Task<IActionResult> GetDirectoratesAsync(int governorateId, CancellationToken ct)
        {
            var result = await locationService.GetDirectoratesByGovernorateAsync(governorateId, ct);
            return Ok(result);
        }

        [HttpGet("neighborhoods/{directorateId}")]
        public async Task<IActionResult> GetNeighborhoodsAsync(int directorateId, CancellationToken ct)
        {
            var result = await locationService.GetNeighborhoodsByDirectorateAsync(directorateId, ct);
            return Ok(result);
        }

        [HttpGet("sub-neighborhoods/{neighborhoodId}")]
        public async Task<IActionResult> GetSubNeighborhoodsAsync(int neighborhoodId, CancellationToken ct)
        {
            var result = await locationService.GetSubNeighborhoodsByNeighborhoodAsync(neighborhoodId, ct);
            return Ok(result);
        }
    }
}
