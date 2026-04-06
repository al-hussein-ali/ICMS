using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Person;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PeopleController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet()]
        public async Task<ActionResult<PagedResult<PersonReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var people = await _personService.GetAllAsync(paginationParams);

            return Ok(people);
        }

        [HttpGet("{id}", Name = "GetPersonById")]
        public async Task<ActionResult<PersonReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var person = await _personService.GetByIdAsync(id);

            return Ok(person);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddAsync(PersonCreateDto dto)
        {

            var newPerson = await _personService.AddAsync(dto);

            return CreatedAtRoute("GetPersonById", new { id = newPerson.Id }, newPerson);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, PersonCreateDto dto)
        {
            await _personService.UpdateAsync(id, dto);

            return Accepted();
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await _personService.DeleteAsync(id);

            return Ok("The record was deleted successfully");
        }
    }
}
