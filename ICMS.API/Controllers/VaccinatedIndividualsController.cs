using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ICMS.API.Extensions;

namespace ICMS.API.Controllers
{
    [Route("api/vaccinated-individuals")]
    [ApiController]
    [Authorize(Roles = Roles.StaffRoles)]
    public class VaccinatedIndividualsController(
        IVaccinatedIndividualService vaccinatedIndividualService,
        ILocalizer localizer) : ControllerBase
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
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto)
        {
            await vaccinatedIndividualService.UpdateAsync(id, vaccinatedIndividualCreateDto);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromQuery] bool deletePersonalInfo = false, [FromQuery] bool isSoftDelete = true)
        {
            await vaccinatedIndividualService.DeleteAsync(id, deletePersonalInfo, isSoftDelete);

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

            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await vaccinatedIndividualService.GiveDose(dto, userId);

            return Accepted();
        }

        [HttpPost("bulk/create")]
        public async Task<IActionResult> BulkInsertNewIndividuals([FromBody] List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals)
        {
            if (!newFieldVaccinatedIndividuals.Any())
                return BadRequest("No records were found.");

            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            var result = await vaccinatedIndividualService.BulkInsertIndividualAsync(newFieldVaccinatedIndividuals, userId);

            // Localize failure messages before returning to mobile app
            if (result.Failures.Any())
            {
                result.Failures = result.Failures.Select(f => f with { Error = localizer[f.Error] }).ToList();
            }

            return Ok(result);
        }

        [HttpPost("bulk/update")]
        public async Task<IActionResult> BulkUpdateFieldVisitIndividuals([FromBody] List<UpdateFieldVisitIndividualDto> updateFieldVisitIndividuals)
        {
            if (!updateFieldVisitIndividuals.Any())
                return BadRequest("No records were found.");

            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            var result = await vaccinatedIndividualService.BulkUpdateFieldVisitIndividualAsync(updateFieldVisitIndividuals, userId);

            // Localize failure messages before returning to mobile app
            if (result.Failures.Any())
            {
                result.Failures = result.Failures.Select(f => f with { Error = localizer[f.Error] }).ToList();
            }

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
