using ICMS.Application.DTOs.ImmunizationRecord;
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

        [HttpGet("byId/{id}", Name = "GetVaccintedIndividualById")]
        public async Task<ActionResult<VaccinatedIndividualDetailsDto>> GetByIdAsync([FromRoute] int id)
        {
            var existingIndividual = await vaccinatedIndividualService.GetByIdAsync(id);

            return Ok(existingIndividual);
        }


        [HttpGet("byCardNumber/{cardNumber}",Name = "GetVaccinatedIndividualByCardNumber")]
        public async Task<ActionResult<VaccinatedIndividualDetailsDto>> GetByCardNumberAsync([FromRoute] string cardNumber)
        {
            var vaccinatedIndividual = await vaccinatedIndividualService.GetByCardNumberAsync(cardNumber);

            return Ok(vaccinatedIndividual);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await vaccinatedIndividualService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("/takeDose")]
        public async Task<IActionResult> TakeDose([FromBody] ImmunizationRecordCreateDto dto)
        {
            await vaccinatedIndividualService.GiveDose(dto);

            return Accepted();
        }

        [HttpPost("new-Individuals")]
        public async Task<IActionResult> BulkInsertNewIndividuals([FromBody] List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals)
        {
            if (newFieldVaccinatedIndividuals == null || !newFieldVaccinatedIndividuals.Any())
                return BadRequest("No records were found.");

            var result = await vaccinatedIndividualService.BulkInsertIndividualAsync(newFieldVaccinatedIndividuals);

            return Ok(result);
        }
    }
}
