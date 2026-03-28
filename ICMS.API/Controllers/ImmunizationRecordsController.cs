using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImmunizationRecordsController : ControllerBase
    {
        private readonly IImmunizationRecordService _immunizationRecordService;
        public ImmunizationRecordsController(IImmunizationRecordService immunizationRecordService)
        {
            _immunizationRecordService = immunizationRecordService;
        }

        [HttpGet()]
        public async Task<ActionResult<PagedResult<ImmunizationRecordReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var immnuizationRecords = await _immunizationRecordService.GetAllAsync(paginationParams);

            return Ok(immnuizationRecords);
        }

        [HttpGet("{id}", Name = "GetImmunizationRecordById")]
        public async Task<ActionResult<ImmunizationRecordReadDto>> GetByIdAsync([FromRoute] Guid id)
        {
            var immunizationRecord = await _immunizationRecordService.GetByIdAsync(id);

            return Ok(immunizationRecord);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ImmunizationRecordCreateDto dto)
        {
            await _immunizationRecordService.UpdateAsync(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await _immunizationRecordService.DeleteAsync(id);

            return NoContent();
        }

    }
}
