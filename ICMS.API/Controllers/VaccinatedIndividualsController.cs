using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinatedIndividualsController(IVaccinatedIndividualService vaccinatedIndividualService) : ControllerBase
    {


        [HttpGet]
        public async Task<ActionResult<PagedResult<VaccinatedIndividualReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var individuals = await vaccinatedIndividualService.GetAllAsync(paginationParams);

            return Ok(individuals);
        }

        [HttpGet("{id}",Name = "GetVaccintedIndividualById")]
        public async Task<ActionResult<VaccinatedIndividualReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var existingIndividual = await vaccinatedIndividualService.GetByIdAsync(id);

            return Ok(existingIndividual);
        }


        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto)
        {
            var newIndividual = await vaccinatedIndividualService.AddAsync(vaccinatedIndividualCreateDto);

            return CreatedAtRoute("GetVaccintedIndividualById", new { id = newIndividual.Id }, newIndividual);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id,VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto)
        {
            await vaccinatedIndividualService.UpdateAsync(id, vaccinatedIndividualCreateDto);

            return NoContent();
        }
    }
}
