using ICMS.Application.DTOs.Vaccine;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinesController(IVaccineService vaccineService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VaccineReadDto>>> GetAllAsync()
        {
            var vaccines = await vaccineService.GetAllAsync();

            return Ok(vaccines);
        }


        [HttpGet("{id}", Name = "GetVaccineById")]
        public async Task<ActionResult<VaccineReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var vaccine = await vaccineService.GetByIdAsync(id);

            return Ok(vaccine);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] VaccineCreateDto dto)
        {
            var newVaccine = await vaccineService.AddAsync(dto);

            return CreatedAtRoute("GetVaccineById", new { id = newVaccine.Id }, newVaccine);
        }


        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAsync([FromRoute] int id,[FromBody] VaccineCreateDto dto)
        {
            await vaccineService.UpdateAsync(id, dto);

            return NoContent();
        }


        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateAsync([FromRoute] int id)
        {
            await vaccineService.DeactivateAsync(id);

            return NoContent();
        }


        [HttpPut("reactivate/{id}")]
        public async Task<IActionResult> ReactivateAsync([FromRoute] int id)
        {
            await vaccineService.ReactivateAsync(id);

            return NoContent();
        }
    }
}
