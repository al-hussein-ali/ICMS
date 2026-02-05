using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class HealthAdvisoryExtensions
    {
        public static HealthAdvisoryReadDto ToReadDto(this HealthAdvisory ha)
            => new(ha.Id, ha.Title, ha.Content, ha.Target, ha.CreationDate);

        public static HealthAdvisoryDetailsDto ToDetailsDto(this HealthAdvisory ha)
            => new(ha.Id, ha.Title, ha.Content, ha.Target, ha.CreationDate, ha.UserId);

        public static HealthAdvisory ToDomain(this HealthAdvisoryCreateDto dto)
            => HealthAdvisory.Create(dto.Title,dto.Content,dto.Target,dto.UserId); // factory not implemented
    }
}
