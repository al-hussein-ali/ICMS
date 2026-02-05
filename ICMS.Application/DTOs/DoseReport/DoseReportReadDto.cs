using System;

namespace ICMS.Application.DTOs.DoseReport
{
    public record DoseReportReadDto(int Id, int BatchId, int UserId, DateTime CreatedAt);
}
