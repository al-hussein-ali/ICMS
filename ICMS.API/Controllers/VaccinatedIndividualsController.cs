using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/vaccinated-individuals")]
    [ApiController]
    public class VaccinatedIndividualsController(IVaccinatedIndividualService vaccinatedIndividualService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<PagedResult<VaccinatedIndividualReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var individuals = await vaccinatedIndividualService.GetAllAsync(paginationParams);

            return Ok(individuals);
        }

        [HttpGet("{id}", Name = "GetVaccinatedIndividualById")]
        public async Task<ActionResult<VaccinatedIndividualDetailsDto>> GetByIdAsync([FromRoute] int id)
        {
            var existingIndividual = await vaccinatedIndividualService.GetByIdAsync(id);

            return Ok(existingIndividual);
        }


        [HttpGet("card/{cardNumber}", Name = "GetVaccinatedIndividualByCardNumber")]
        public async Task<ActionResult<VaccinatedIndividualDetailsDto>> GetByCardNumberAsync([FromRoute] string cardNumber)
        {
            var vaccinatedIndividual = await vaccinatedIndividualService.GetByCardNumberAsync(cardNumber);

            return Ok(vaccinatedIndividual);
        }


        [HttpPost("create")]
        public async Task<IActionResult> AddAsync([FromBody] VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto)
        {
            var newIndividual = await vaccinatedIndividualService.AddAsync(vaccinatedIndividualCreateDto);

            return CreatedAtRoute("GetVaccinatedIndividualById", new { id = newIndividual.Id }, newIndividual);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto)
        {
            await vaccinatedIndividualService.UpdateAsync(id, vaccinatedIndividualCreateDto);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await vaccinatedIndividualService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("{individualId}/vaccinations/create")]
        public async Task<IActionResult> TakeDose([FromRoute] int individualId, [FromBody] ImmunizationRecordCreateDto dto)
        {
            // Ensure ID from route matches body or just use route ID
            if (individualId != dto.IndividualId)
            {
                return BadRequest("ID mismatch");
            }

            await vaccinatedIndividualService.GiveDose(dto);

            return Accepted();
        }

        [HttpPost("bulk/create")]
        public async Task<IActionResult> BulkInsertNewIndividuals([FromBody] List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals)
        {
            if (!newFieldVaccinatedIndividuals.Any())
                return BadRequest("No records were found.");

            var result = await vaccinatedIndividualService.BulkInsertIndividualAsync(newFieldVaccinatedIndividuals);

            return Ok(result);
        }

        [HttpPost("bulk/update")]
        public async Task<IActionResult> BulkUpdateFieldVisitIndividuals([FromBody] List<UpdateFieldVisitIndividualDto> updateFieldVisitIndividuals)
        {
            if (!updateFieldVisitIndividuals.Any())
                return BadRequest("No records were found.");

            var result = await vaccinatedIndividualService.BulkUpdateFieldVisitIndividualAsync(updateFieldVisitIndividuals);

            return Ok(result);
        }

        [HttpPost("{id}/account")]
        public async Task<IActionResult> GenerateAccount([FromRoute] int id)
        {
            var result = await vaccinatedIndividualService.GenerateAccountAsync(id);
            return Ok(result);
        }
    }
}
