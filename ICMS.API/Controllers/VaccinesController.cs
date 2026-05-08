using ICMS.Application.DTOs.Vaccine;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/vaccines")]
    [ApiController]
    [Authorize(Roles = Roles.StaffRoles)]
    public class VaccinesController(IVaccineService vaccineService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VaccineReadDto>>> GetAllAsync([FromQuery] string? name = null)
        {
            var vaccines = await vaccineService.GetAllAsync(name);
            return Ok(vaccines);
        }

        [HttpGet("{id}", Name = "GetVaccineById")]
        public async Task<ActionResult<VaccineReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var vaccine = await vaccineService.GetByIdAsync(id);

            return Ok(vaccine);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddAsync([FromBody] VaccineCreateDto dto)
        {
            var newVaccine = await vaccineService.AddAsync(dto);

            return CreatedAtRoute("GetVaccineById", new { id = newVaccine.Id }, newVaccine);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] VaccineCreateDto dto)
        {
            await vaccineService.UpdateAsync(id, dto);

            return NoContent();
        }


        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAsync([FromRoute] int id)
        {
            await vaccineService.DeactivateAsync(id);

            return NoContent();
        }


        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> ReactivateAsync([FromRoute] int id)
        {
            await vaccineService.ReactivateAsync(id);

            return NoContent();
        }
    }
}
