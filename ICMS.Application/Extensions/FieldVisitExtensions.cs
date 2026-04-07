using ICMS.Application.DTOs.FieldVisit;
using ICMS.Domain.Entites.Visits;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class FieldVisitExtensions
    {
        public static FieldVisitReadDto ToReadDto(this FieldVisit fv)
        {
            return new FieldVisitReadDto(
                fv.Id,
                fv.VisitDate,
                fv.SubNeighborhoodId,
                fv.SubNeighborhood?.Name ?? string.Empty,
                fv.IsCompleted
            );
        }

        public static FieldVisitDetailsDto ToDetailsDto(this FieldVisit fv)
        {
            return new FieldVisitDetailsDto(
                fv.Id,
                fv.VisitDate,
                fv.SubNeighborhoodId,
                fv.SubNeighborhood?.Name ?? string.Empty,
                fv.IsCompleted,
                fv.FieldVisitUsers.Select(fvu => new FieldWorkerDto(fvu.UserId, fvu.FieldVisitId)).ToList(),
                fv.ImmunizationRecords.Count
            );
        }
    }
}
