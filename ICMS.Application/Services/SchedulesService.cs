using ICMS.Application.Extensions;
using ICMS.Application.DTOs.Schedules;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class SchedulesService : ISchedulesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SchedulesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ScheduleReadDto>> GetIndividualSchedulesAsync(int individualId, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository
                .GetIndividualWithSchedulesAsync(individualId, ct);

            if (individual == null)
            {
                throw new NotFoundException($"Individual with ID {individualId} not found.");
            }

            return individual.Schedules.Select(s => s.ToReadDto()).ToList();
        }

        public async Task<IEnumerable<MissedScheduleReadDto>> GetMissedSchedulesDetailedAsync(MissedScheduleQueryDto query, CancellationToken ct = default)
        {
            var toDate = query.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));

            return await _unitOfWork.VaccinationScheduleRepository
                .GetMissedSchedulesDetailedAsync(query.FromDate, toDate, ct);
        }
    }
}
