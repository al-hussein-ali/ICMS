using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchFilterDto(int? DoseId, DateOnly? ExpiryDate);
}
