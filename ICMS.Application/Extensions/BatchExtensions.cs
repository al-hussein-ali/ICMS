using ICMS.Application.DTOs.Batch;
using ICMS.Domain.Entites;

namespace ICMS.Application.Extensions
{
    public static class BatchExtensions
    {
        public static BatchReadDto ToReadDto(this Batch b)
            => new(b.Id, b.DoseId, b.UserId, b.ExpiryDate, b.TotalQuantity);

        public static BatchDetailsDto ToDetailsDto(this Batch b)
            => new(b.Id, b.DoseId, b.UserId, b.ExpiryDate, b.TotalQuantity, b.CountryOfOrigin, b.CookNumber, b.Notes);

        public static Batch ToDomain(this BatchCreateDto dto)
            => Batch.Create(dto.DoseId, dto.UserId, dto.ExpiryDate, dto.TotalQuantity, dto.Notes ?? string.Empty, null, dto.Notes);
    }
}
