using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IBatchExpirationTrackerService
    {
        /// <summary>
        /// Checks for batches that are soon to expire and sends notifications.
        /// </summary>
        Task TrackExpiringBatchesAsync(CancellationToken ct = default);
    }
}
