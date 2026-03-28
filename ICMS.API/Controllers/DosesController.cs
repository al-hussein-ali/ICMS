using ICMS.Application.DTOs.Dose;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DosesController(IDoseService doseService) : ControllerBase
    {
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<DoseReadDto>))]
        public async Task<ActionResult<IReadOnlyList<DoseReadDto>>> GetAllAsync([FromQuery] int? vaccineId)
        {
            var dosages = await doseService.GetAllAsync(vaccineId);

            return Ok(dosages);
        }

        [HttpGet("byId/{id}", Name = "GetById")]
        public async Task<ActionResult<DoseReadDto>> GetByNameAsync([FromRoute] int id)
        {
            var dose = await doseService.GetByIdAsync(id);

            return Ok(dose);
        }

        [HttpGet("byName/{name}", Name = "GetByName")]
        public async Task<ActionResult<DoseReadDto>> GetByNameAsync([FromRoute] string name)
        {
            var dose = await doseService.GetByNameAsync(name);

            return Ok(dose);
        }

        [HttpPost()]
        public async Task<IActionResult> AddAsync([FromBody] DoseCreateDto dto)
        {
            var newDose = await doseService.AddAsync(dto);

            return CreatedAtRoute("GetById", new { id = newDose.Id }, newDose);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] DoseCreateDto dto)
        {
            await doseService.UpdateAsync(id, dto);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await doseService.DeleteAsync(id);

            return NoContent();
        }

    }
}
