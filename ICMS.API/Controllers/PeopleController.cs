using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Person;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/people")]
    [ApiController]
    [Authorize(Roles = Roles.Admin + "," + Roles.VaccinationManager + "," + Roles.ReproductiveHealthManager)]
    public class PeopleController(IPersonService personService) : ControllerBase
    {

        [HttpGet()]
        public async Task<ActionResult<PagedResult<PersonReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var people = await personService.GetAllAsync(paginationParams);

            return Ok(people);
        }

        [HttpGet("{id}", Name = "GetPersonById")]
        public async Task<ActionResult<PersonReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var person = await personService.GetByIdAsync(id);

            return Ok(person);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddAsync(PersonCreateDto dto)
        {

            var newPerson = await personService.AddAsync(dto);

            return CreatedAtRoute("GetPersonById", new { id = newPerson.Id }, newPerson);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, PersonCreateDto dto)
        {
            await personService.UpdateAsync(id, dto);

            return Accepted();
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await personService.DeleteAsync(id);

            return Ok("The record was deleted successfully");
        }

        [HttpGet("search/name")]
        public async Task<ActionResult<List<PersonReadDto>>> SearchByName([FromQuery] string fullName)
        {
            var results = await personService.GetByName(fullName);
            return Ok(results);
        }

        [HttpGet("search/phone")]
        public async Task<ActionResult<List<PersonReadDto>>> SearchByPhone([FromQuery] string phoneNumber)
        {
            var results = await personService.GetByPhone(phoneNumber);
            return Ok(results);
        }
    }
}
