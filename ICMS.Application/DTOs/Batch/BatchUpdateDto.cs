using System;

namespace ICMS.Application.DTOs.Batch
{
    public record BatchUpdateDto(string BatchName, string CountryOfOrigin, string CookNumber, DateOnly ExpiryDate, string? Notes);
}
