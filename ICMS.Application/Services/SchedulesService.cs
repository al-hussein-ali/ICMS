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
        private readonly ICacheService _cacheService;

        public SchedulesService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ScheduleReadDto>> GetIndividualSchedulesAsync(int individualId, CancellationToken ct = default)
        {
            var lang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            string cacheKey = $"schedules:individual:{individualId}:{lang}";
            if (_cacheService.TryGet(cacheKey, out IEnumerable<ScheduleReadDto>? cached) && cached != null)
                return cached;

            var individual = await _unitOfWork.VaccinatedIndividualRepository
                .GetIndividualWithSchedulesAsync(individualId, ct);

            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            var dtos = individual.Schedules.Select(s => s.ToReadDto()).ToList();
            _cacheService.Set(cacheKey, dtos, TimeSpan.FromMinutes(30));
            return dtos;
        }

        public async Task<IEnumerable<MissedScheduleReadDto>> GetMissedSchedulesDetailedAsync(MissedScheduleQueryDto query, CancellationToken ct = default)
        {
            var toDate = query.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));

            return await _unitOfWork.VaccinationScheduleRepository
                .GetMissedSchedulesDetailedAsync(query.FromDate, toDate, query.SubNeighborhoodId, query.WorkerId, ct);
        }
    }
}
