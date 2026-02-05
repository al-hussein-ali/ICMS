using ICMS.Application.DTOs.DoseReport;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class DoseReportExtensions
    {
        public static DoseReportReadDto ToReadDto(this DoseReport dr)
            => new(dr.Id, dr.BatchId, dr.UserId, dr.CreatedAt);

        public static DoseReportDetailsDto ToDetailsDto(this DoseReport dr)
            => new(dr.Id, dr.BatchId, dr.UserId, dr.CreatedAt, dr.Description);

        public static DoseReport ToDomain(this DoseReportCreateDto dto)
            => DoseReport.Create(dto.BatchId,dto.UserId,dto.Description); // factory not implemented
    }
}
