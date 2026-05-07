using ICMS.API.Extensions;
using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ICMS.Domain.ValueObjects;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/batches")]
    [Authorize(Roles = Roles.Admin + "," + Roles.InventoryManager)]
    public class BatchesController(IBatchService batchService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<BatchReadDto>>> GetAllAsync([FromQuery] BatchFilterDto filter, [FromQuery] PaginationParams paginationParams)
        {
            var result = await batchService.GetAllAsync(filter, paginationParams);
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<PagedResult<TransactionReadDto>>> GetAllTransactionsAsync([FromQuery] TransactionFilterDto filter, [FromQuery] PaginationParams paginationParams)
        {
            var result = await batchService.GetAllTransactionsAsync(filter, paginationParams);
            return Ok(result);
        }

        [HttpGet("{batchId}", Name = "GetBatchById")]
        public async Task<ActionResult<BatchDetailsDto>> GetByIdAsync([FromRoute] int batchId)
        {
            var result = await batchService.GetByIdAsync(batchId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BatchReadDto>> AddAsync([FromBody] BatchCreateDto dto)
        {
            var userId = User.GetUserId();
            var result = await batchService.AddAsync(dto, userId);
            return CreatedAtRoute("GetBatchById", new { batchId = result.Id }, result);
        }

        [HttpPut("{batchId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int batchId, [FromBody] BatchUpdateDto dto)
        {
            await batchService.UpdateAsync(batchId, dto);
            return NoContent();
        }

        [HttpPost("{batchId}/inventory/add")]
        public async Task<IActionResult> AddStockAsync([FromRoute] int batchId, [FromBody] InventoryUpdateDto dto)
        {
            var userId = User.GetUserId();
            await batchService.AddStockAsync(batchId, dto, userId);
            return Ok();
        }

        [HttpPost("{batchId}/inventory/remove")]
        public async Task<IActionResult> RemoveStockAsync([FromRoute] int batchId, [FromBody] InventoryUpdateDto dto)
        {
            var userId = User.GetUserId();
            await batchService.RemoveStockAsync(batchId, dto, userId);
            return Ok();
        }

        [HttpGet("{batchId}/transactions")]
        public async Task<ActionResult<PagedResult<TransactionReadDto>>> GetTransactionsAsync([FromRoute] int batchId, [FromQuery] TransactionFilterDto filter, [FromQuery] PaginationParams paginationParams)
        {
            var result = await batchService.GetTransactionsAsync(batchId, filter, paginationParams);
            return Ok(result);
        }

        [HttpDelete("{batchId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int batchId)
        {
            await batchService.DeactivateAsync(batchId);
            return NoContent();
        }
    }
}
