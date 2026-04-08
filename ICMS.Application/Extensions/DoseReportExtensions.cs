using ICMS.Application.DTOs.DoseReport;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class DoseReportExtensions
    {
        public static DoseReportReadDto ToReadDto(this DoseReport dr)
            => new(dr.Id, dr.BatchId, dr.UserId, dr.CreatedAt);

        public static DoseReportDetailsDto ToDetailsDto(this DoseReport dr)
            => new(dr.Id, dr.BatchId, dr.UserId, dr.CreatedAt, dr.Description);

        public static DoseReport ToDomain(this DoseReportCreateDto dto, int userId)
            => DoseReport.Create(dto.BatchId, userId, dto.Description); // factory not implemented
    }
}
