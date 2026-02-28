using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImmunizationRecordsController : ControllerBase
    {
        private readonly ImmunizationRecordService _immunizationRecordService; 
        public ImmunizationRecordsController(ImmunizationRecordService immunizationRecordService)
        {
            _immunizationRecordService = immunizationRecordService;
        }

        [HttpGet()]

        public async Task<ActionResult<PagedResult<ImmunizationRecordReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var immnuizationRecords = await _immunizationRecordService.GetAllAsync(paginationParams);

            return Ok(immnuizationRecords);
        }


        [HttpGet("{id}",Name = "GetImmunizationRecordByID")]
        public async Task<ActionResult<ImmunizationRecordReadDto>> GetByIdAsync([FromRoute] Guid id)
        {
            var immunizationRecord = await _immunizationRecordService.GetByIdAsync(id);

            return Ok(immunizationRecord);
        }

    }
}
