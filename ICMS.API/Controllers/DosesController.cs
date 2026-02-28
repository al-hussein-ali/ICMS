using ICMS.Application.DTOs.Dose;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DosesController(IDoseService doseService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<IReadOnlyList<DoseReadDto>>> GetAllAsync([FromRoute] int? vaccineId)
        {
            var dosages = await doseService.GetAllAsync(vaccineId);

            return Ok(dosages);
        }

        [HttpGet("byId/{id}", Name = "GetById")]
        public async Task<ActionResult<DoseReadDto>> GetByNameAsync([FromRoute] int id)
        {
            var dose = await doseService.GetByIdAsync(id);

            return Ok(dose);
        }

        [HttpGet("byName/{name}", Name = "GetByName")]
        public async Task<ActionResult<DoseReadDto>> GetByNameAsync([FromRoute] string name)
        {
            var dose = await doseService.GetByNameAsync(name);

            return Ok(dose);
        }

 
    }
}
