using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IFieldVisitReminderService
    {
        Task<bool> SendRemindersForVisitAsync(int fieldVisitId, CancellationToken ct = default);
        Task SendScheduledRemindersAsync(CancellationToken ct = default);
    }
}
