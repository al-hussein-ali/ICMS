using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class HealthAdvisoryExtensions
    {
        public static HealthAdvisoryReadDto ToReadDto(this HealthAdvisory ha)
            => new(ha.Id, ha.Title, ha.Content, ha.Target, ha.ScheduledDate, ha.IsSent, ha.CreationDate);

        public static HealthAdvisoryDetailsDto ToDetailsDto(this HealthAdvisory ha)
            => new(ha.Id, ha.Title, ha.Content, ha.Target, ha.ScheduledDate, ha.IsSent, ha.CreationDate, ha.UserId);

        public static HealthAdvisory ToDomain(this HealthAdvisoryCreateDto dto, int userId)
            => HealthAdvisory.Create(dto.Title, dto.Content, dto.Target, dto.ScheduledDate, userId);
    }
}
