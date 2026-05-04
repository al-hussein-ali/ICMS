using ICMS.Application.DTOs.FieldVisit;
using ICMS.Domain.Entites.Visits;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class FieldVisitExtensions
    {
        public static FieldVisitReadDto ToReadDto(this FieldVisit fv)
        {
            string location = "Unknown";
            if (fv.SubNeighborhood != null && fv.SubNeighborhood.Neighborhood?.Directorate?.Governorate != null)
            {
                location = $"{fv.SubNeighborhood.Neighborhood.Directorate.Governorate.Name} - {fv.SubNeighborhood.Neighborhood.Directorate.Name} - {fv.SubNeighborhood.Neighborhood.Name} - {fv.SubNeighborhood.Name}";
            }
            else if (fv.SubNeighborhood != null)
            {
                location = fv.SubNeighborhood.Name;
            }

            return new FieldVisitReadDto(
                fv.Id,
                fv.CampaignName,
                fv.VisitDate,
                fv.SubNeighborhood?.Neighborhood?.DirectorateId ?? 0,
                fv.SubNeighborhood?.NeighborhoodId ?? 0,
                fv.SubNeighborhoodId,
                location,
                0, 
                fv.IsCompleted
            );
        }

        public static FieldVisitDetailsDto ToDetailsDto(this FieldVisit fv)
        {
            return new FieldVisitDetailsDto(
                fv.Id,
                fv.CampaignName,
                fv.VisitDate,
                fv.SubNeighborhoodId,
                fv.SubNeighborhood?.Name ?? "N/A",
                fv.SubNeighborhood?.NeighborhoodId ?? 0,
                fv.SubNeighborhood?.Neighborhood?.DirectorateId ?? 0,
                fv.SubNeighborhood?.Neighborhood?.Directorate?.GovernorateId ?? 0,
                fv.FromDate,
                fv.ToDate,
                fv.IsCompleted,
                fv.ImmunizationRecords.Count
            );
        }
    }
}
