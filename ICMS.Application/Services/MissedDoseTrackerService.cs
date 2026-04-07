using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ICMS.Application.Services;

public class MissedDoseTrackerService : IMissedDoseTrackerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MissedDoseTrackerService> _logger;

    public MissedDoseTrackerService(IUnitOfWork unitOfWork, ILogger<MissedDoseTrackerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task MarkMissedDosesAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting missed dose tracker at {Time} (UTC)", DateTime.UtcNow);

        // ICMS operating timezone is UTC+3 (aligned with user's local time)
        var localNow = DateTime.UtcNow.AddHours(3);
        var cutoffDate = DateOnly.FromDateTime(localNow);

        _logger.LogInformation("Identifying pending doses scheduled before {CutoffDate}", cutoffDate);

        // Find all pending schedules where the scheduled date is before today (local)
        var overdueSchedules = await _unitOfWork.VaccinationScheduleRepository
            .GetOverduePendingSchedulesAsync(cutoffDate, ct);

        if (!overdueSchedules.Any())
        {
            _logger.LogInformation("No overdue pending doses found.");
            return;
        }

        _logger.LogInformation("Found {Count} overdue doses. Marking as Missed.", overdueSchedules.Count);

        foreach (var schedule in overdueSchedules)
        {
            schedule.MarkAsMissed();
        }

        var result = await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Successfully updated {Count} vaccination schedules to Missed status.", result);
    }
}
