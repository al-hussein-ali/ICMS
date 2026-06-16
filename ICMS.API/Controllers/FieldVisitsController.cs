using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using ICMS.Domain.ValueObjects;
using ICMS.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Api.Controllers
{
    [Route("api/field-visits")]
    [ApiController]
    [Authorize(Roles = Roles.StaffRoles)]
    public class FieldVisitsController(
        IFieldVisitService fieldVisitService, 
        IUserService userService,
        IFieldVisitReminderService fieldVisitReminderService,
        ICMS.Infrastructure.Persistence.Data.AppDbContext dbContext) : ControllerBase
    {
        [HttpGet("diagnostic-db")]
        [AllowAnonymous]
        public async Task<IActionResult> DiagnosticDb(CancellationToken ct)
        {
            var visits = await dbContext.FieldVisits
                .Select(fv => new {
                    fv.Id,
                    fv.CampaignName,
                    fv.IsCompleted,
                    IndividualsCount = dbContext.FieldVisitIndividuals.Count(fvi => fvi.FieldVisitId == fv.Id),
                    WorkersCount = dbContext.FieldVisitWorkers.Count(fvw => fvw.FieldVisitId == fv.Id)
                })
                .ToListAsync(ct);

            var totalIndividuals = await dbContext.FieldVisitIndividuals.CountAsync(ct);
            var totalWorkers = await dbContext.FieldVisitWorkers.CountAsync(ct);

            var individuals = await dbContext.VaccinatedIndividuals
                .Select(vi => new { vi.Id, vi.CardNumber })
                .Take(5)
                .ToListAsync(ct);

            var subNeighborhoods = await dbContext.SubNeighborhoods
                .Select(sn => new { sn.Id, sn.Name })
                .Take(5)
                .ToListAsync(ct);

            var workers = await dbContext.Users
                .Select(u => new { u.Id, u.UserName })
                .Take(5)
                .ToListAsync(ct);

            return Ok(new {
                TotalVisits = visits.Count,
                TotalIndividualsInRelations = totalIndividuals,
                TotalWorkersInRelations = totalWorkers,
                Visits = visits,
                SampleIndividuals = individuals,
                SampleSubNeighborhoods = subNeighborhoods,
                SampleWorkers = workers
            });
        }

        [HttpGet("workers")]
        public async Task<ActionResult<IReadOnlyList<ICMS.Application.DTOs.User.UserReadDto>>> GetAvailableWorkersAsync(CancellationToken ct)
        {
            var workers = await userService.GetAvailableFieldWorkersAsync(ct);
            return Ok(workers);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<FieldVisitReadDto>>> GetAllAsync(
            [FromQuery] PaginationParams paginationParams, [FromQuery] bool? onlyUncompleted, CancellationToken ct)
        {
            try
            {
                await fieldVisitService.CloseExpiredVisitsAsync(ct);
            }
            catch (Exception)
            {
                // Log and continue so the API list load doesn't crash on DB transient errors
            }

            int? workerId = null;
            if (User.IsInRole(Roles.FieldVisitWorker) && 
                !User.IsInRole(Roles.Admin) && 
                !User.IsInRole(Roles.VaccinationManager) && 
                !User.IsInRole(Roles.InventoryManager) && 
                !User.IsInRole(Roles.ReproductiveHealthManager))
            {
                workerId = User.GetUserId();
                onlyUncompleted = true;
            }

            var fieldVisits = await fieldVisitService.GetAllAsync(paginationParams, onlyUncompleted: onlyUncompleted, workerId: workerId, ct: ct);
            return Ok(fieldVisits);
        }

        [HttpGet("{id}", Name = "GetFieldVisitById")]
        public async Task<ActionResult<FieldVisitDetailsDto>> GetByIdAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            var fieldVisit = await fieldVisitService.GetByIdAsync(id, ct);
            return Ok(fieldVisit);
        }

        /// <summary>
        /// GET /api/field-visits/{id}/vaccinations
        /// Returns a vaccination summary for the given field visit:
        ///   - VaccinatedPersons  → list of targeted individuals with their pending doses
        ///   - AdministeredBy     → list of field workers assigned to the visit
        /// </summary>
        [HttpGet("{id}/vaccinations")]
        public async Task<ActionResult<FieldVisitVaccinationsDto>> GetVaccinationsAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            var result = await fieldVisitService.GetVaccinationsAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAsync(
            [FromBody] FieldVisitCreateDto dto, CancellationToken ct)
        {
            Console.WriteLine($">>> FieldVisitsController.AddAsync: CampaignName={dto.CampaignName}, SelectedIndividualIds.Count={dto.SelectedIndividualIds?.Count ?? -1}, SelectedWorkerIds.Count={dto.SelectedWorkerIds?.Count ?? -1}");
            if (dto.SelectedIndividualIds != null)
            {
                Console.WriteLine($">>> Individual IDs: {string.Join(",", dto.SelectedIndividualIds)}");
            }
            if (dto.SelectedWorkerIds != null)
            {
                Console.WriteLine($">>> Worker IDs: {string.Join(",", dto.SelectedWorkerIds)}");
            }
            var created = await fieldVisitService.AddAsync(dto, ct);
            return CreatedAtRoute("GetFieldVisitById", new { id = created.Id }, created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id, [FromBody] FieldVisitCreateDto dto, CancellationToken ct)
        {
            await fieldVisitService.UpdateAsync(id, dto, ct);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            await fieldVisitService.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> MarkCompletedAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            await fieldVisitService.MarkCompletedAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/send-reminders")]
        public async Task<IActionResult> SendRemindersAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            var success = await fieldVisitReminderService.SendRemindersForVisitAsync(id, ct);
            if (!success)
            {
                return BadRequest(new { Message = "Failed to send reminders. No targeted users with active devices, or reminders already sent." });
            }
            return Ok(new { Message = "Reminders sent successfully." });
        }

        [HttpPost("{id}/send-worker-notifications")]
        public async Task<IActionResult> SendWorkerNotificationsAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            var success = await fieldVisitService.SendWorkerNotificationsAsync(id, ct);
            if (!success)
            {
                return BadRequest(new { Message = "Failed to send notifications. No workers assigned or no active devices registered." });
            }
            return Ok(new { Message = "Worker notifications sent successfully." });
        }
    }
}
