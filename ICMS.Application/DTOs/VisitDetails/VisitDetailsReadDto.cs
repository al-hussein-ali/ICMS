using System;

namespace ICMS.Application.DTOs.VisitDetails
{
    public record VisitDetailsReadDto(int Id, int PregnancyDetailsId, DateOnly VisitDate, DateOnly? NextVisitDate, decimal WeightInKilo);
}
