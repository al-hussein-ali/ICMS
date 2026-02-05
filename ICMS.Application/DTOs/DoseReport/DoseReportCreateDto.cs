using System;

namespace ICMS.Application.DTOs.DoseReport
{
    public record DoseReportCreateDto(int BatchId, int UserId, string? Description);
}
