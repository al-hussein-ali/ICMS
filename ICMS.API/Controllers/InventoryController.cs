using ICMS.API.Extensions;
using ICMS.Application.DTOs.Batch;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    [Authorize(Roles = Roles.Admin + "," + Roles.InventoryManager)]
    public class InventoryController(IBatchService batchService) : ControllerBase
    {
        [HttpPost("subtract-by-dose")]
        public async Task<IActionResult> SubtractByDoseAsync([FromBody] InventoryRemoveByDoseDto dto)
        {
            var userId = User.GetUserId();
            await batchService.RemoveStockByDoseAsync(dto, userId);
            return Ok();
        }
    }
}
