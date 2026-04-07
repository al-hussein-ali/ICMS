using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services;

public interface IMissedDoseTrackerService
{
    Task MarkMissedDosesAsync(CancellationToken ct = default);
}
