using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.HealthAdvisory
{
    public record HealthAdvisoryReadDto(int Id, string Title, string Content, AdviceTarget Target, DateOnly ScheduledDate, bool IsSent, DateTime CreationDate);
}
