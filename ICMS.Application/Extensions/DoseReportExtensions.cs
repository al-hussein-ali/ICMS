using ICMS.Application.DTOs.DoseReport;
using ICMS.Domain.Entites.Clinical;

namespace ICMS.Application.Extensions
{
    public static class DoseReportExtensions
    {
        public static DoseReportReadDto ToReadDto(this DoseReport dr)
            => new(dr.Id, dr.BatchId, dr.UserId, dr.CreatedAt, dr.Description);

        public static DoseReportDetailsDto ToDetailsDto(this DoseReport dr)
            => new(dr.Id, dr.BatchId, dr.UserId, dr.CreatedAt, dr.Description);

        public static DoseReport ToDomain(this DoseReportCreateDto dto, int userId)
            => DoseReport.Create(dto.BatchId, userId, dto.Description);
    }
}
