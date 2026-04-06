using ICMS.Application.DTOs.Dose;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/doses")]
    public class DosesController(IDoseService doseService) : ControllerBase
    {
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<DoseReadDto>))]
        public async Task<ActionResult<IReadOnlyList<DoseReadDto>>> GetAllAsync([FromQuery] int? vaccineId)
        {
            var dosages = await doseService.GetAllAsync(vaccineId);

            return Ok(dosages);
        }

        [HttpGet("{id}", Name = "GetDoseById")]
        public async Task<ActionResult<DoseReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var dose = await doseService.GetByIdAsync(id);

            return Ok(dose);
        }

        [HttpGet("name/{name}", Name = "GetDoseByName")]
        public async Task<ActionResult<DoseReadDto>> GetByNameAsync([FromRoute] string name)
        {
            var dose = await doseService.GetByNameAsync(name);

            return Ok(dose);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddAsync([FromBody] DoseCreateDto dto)
        {
            var newDose = await doseService.AddAsync(dto);

            return CreatedAtRoute("GetDoseById", new { id = newDose.Id }, newDose);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] DoseCreateDto dto)
        {
            await doseService.UpdateAsync(id, dto);

            return NoContent();
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await doseService.DeleteAsync(id);

            return NoContent();
        }

    }
}
