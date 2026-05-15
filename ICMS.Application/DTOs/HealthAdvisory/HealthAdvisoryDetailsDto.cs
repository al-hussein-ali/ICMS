using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.HealthAdvisory
{
    public record HealthAdvisoryDetailsDto(int Id, string Title, string Content, string? ImageUrl, AdviceTarget Target, DateOnly ScheduledDate, bool IsSent, DateTime CreationDate, int UserId);
}
