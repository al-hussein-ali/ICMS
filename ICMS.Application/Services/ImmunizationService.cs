using ICMS.Application.DTOs.Schedules;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class ImmunizationService : IImmunizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImmunizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AdministerDoseAsync(AdministerDoseDto request, int userId, CancellationToken ct = default)
        {
            // 1. Load Aggregate Root with Includes (_schedules, _immunizationRecords)
            var individual = await _unitOfWork.VaccinatedIndividualRepository
                .GetIndividualWithSchedulesAsync(request.IndividualId, ct);

            if (individual == null)
            {
                throw new NotFoundException($"VaccinatedIndividual with ID {request.IndividualId} not found.");
            }

            // 2. Load Doses (Current and Next for interval calculation)
            var currentDose = await _unitOfWork.DoseRepository.GetByIdAsync(request.DoseId, ct);
            if (currentDose == null)
            {
                throw new NotFoundException($"Dose with ID {request.DoseId} not found.");
            }

            var nextDose = await _unitOfWork.DoseRepository
                .GetNextDoseInSequenceAsync(currentDose.VaccineId, currentDose.DoseOrder, ct);

            // 3. Delegate to Domain Entity
            individual.AdministerDose(
                currentDose, 
                request.Date, 
                request.TakenIn, 
                userId,
                nextDose, 
                request.FieldVisitId, 
                request.Notes);

            // 4. Save
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
