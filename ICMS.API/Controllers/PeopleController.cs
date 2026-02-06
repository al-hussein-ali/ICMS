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
            var people = personService.GetAll(paginationParams);

            return Ok(people);
        }

        [HttpGet("by-Id")]
        public async Task<ActionResult<PersonReadDto>> GetByIdAsync([FromQuery] int id)
        {
            var person = await personService.GetByIdAsync(id);

            return Ok(person);
        }

        [HttpPost]
        public async Task<PersonReadDto> AddAsync(PersonCreateDto dto)
        {
            
            var newPerson = await personService.AddAsync(dto);

            return newPerson;
        }
    }
}
