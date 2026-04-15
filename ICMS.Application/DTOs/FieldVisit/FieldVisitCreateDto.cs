using System;

namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitCreateDto(
        DateOnly VisitDate,
        int SubNeighborhoodId);
}
