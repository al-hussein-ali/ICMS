using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.HealthAdvisory
{
    public record HealthAdvisoryCreateDto(string Title, string Content, AdviceTarget Target, DateOnly? ScheduledDate);
}
