using System;
using ICMS.Domain.Enums;

namespace ICMS.Application.DTOs.HealthAdvisory
{
    public record HealthAdvisoryDetailsDto(int Id, string Title, string Content, AdviceTarget Target, DateTime CreationDate, int UserId);
}
