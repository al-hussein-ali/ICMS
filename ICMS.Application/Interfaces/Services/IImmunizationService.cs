using ICMS.Application.DTOs.Schedules;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IImmunizationService
    {
        Task AdministerDoseAsync(AdministerDoseDto request, CancellationToken ct = default);
    }
}
