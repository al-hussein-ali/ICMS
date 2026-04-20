using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IAdvisoryDispatchBackgroundService
    {
        Task DispatchPendingAdvisoriesAsync(CancellationToken ct = default);
    }
}
