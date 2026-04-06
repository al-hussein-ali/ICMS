using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.User;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var result = await userService.GetAllAsync(paginationParams);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<UserReadDto>> GetByIdAsync([FromRoute] int id)
        {
            var result = await userService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserReadDto>> AddAsync([FromBody] UserCreateDto userCreateDto)
        {
            var result = await userService.AddAsync(userCreateDto);
            return CreatedAtRoute("GetUserById", new { id = result.Id }, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UserReadDto userReadDto)
        {
            if (id != userReadDto.Id) return BadRequest("ID mismatch");

            await userService.UpdateAsync(userReadDto);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var success = await userService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
