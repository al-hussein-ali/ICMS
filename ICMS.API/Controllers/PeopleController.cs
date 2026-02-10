using FluentValidation;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Person;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService personService;

        public PeopleController(IPersonService personService)
        {
            this.personService = personService;
        }

        [HttpGet("all")]
        public ActionResult<PagedResult<PersonReadDto>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var people = personService.GetAllAsync(paginationParams);

            return Ok(people);
        }

        [HttpGet("{id}",Name = "GetPersonById")]
        public async Task<ActionResult<PersonReadDto>> GetByIdAsync([FromQuery] int id)
        {
            var person = await personService.GetByIdAsync(id);

            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(PersonCreateDto dto)
        {

            var newPerson = await personService.AddAsync(dto);

            return CreatedAtRoute("GetPersonById", new { id = newPerson.Id }, newPerson);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromQuery] int id, PersonCreateDto dto)
        {
            await personService.UpdateAsync(id, dto);

            return Accepted();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromQuery] int id)
        {
            await personService.DeleteAsync(id);

            return Ok("The record was deleted successfully");
        }
    }
}
