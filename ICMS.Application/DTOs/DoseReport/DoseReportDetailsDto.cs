using System;

namespace ICMS.Application.DTOs.DoseReport
{
    public record DoseReportDetailsDto(int Id, int BatchId, int UserId, DateTime CreatedAt, string? Description);
}
