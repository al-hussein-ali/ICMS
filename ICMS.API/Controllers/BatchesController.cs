using ICMS.API.Extensions;
using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/batches")]
    [Authorize]
    public class BatchesController(IBatchService batchService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<BatchReadDto>>> GetAllAsync([FromQuery] BatchFilterDto filter, [FromQuery] PaginationParams paginationParams)
        {
            var result = await batchService.GetAllAsync(filter, paginationParams);
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
            return CreatedAtRoute("GetBatchById", new { id = result.Id }, result);
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
        public async Task<ActionResult<PagedResult<TransactionReadDto>>> GetTransactionsAsync([FromRoute] int batchId, [FromQuery] PaginationParams paginationParams)
        {
            var result = await batchService.GetTransactionsAsync(batchId, paginationParams);
            return Ok(result);
        }
    }
}
