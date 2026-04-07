using System;
using System.Collections.Generic;

namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitCreateDto(
        DateOnly VisitDate,
        int SubNeighborhoodId,
        List<int>? FieldWorkerUserIds = null);
}
