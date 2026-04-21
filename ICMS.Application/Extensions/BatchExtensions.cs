using ICMS.Application.DTOs.Batch;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Extensions
{
    public static class BatchExtensions
    {
        public static BatchReadDto ToReadDto(this Batch b)
            => new(b.Id, b.DoseId, b.UserId, b.ExpiryDate, b.TotalQuantity);

        public static BatchDetailsDto ToDetailsDto(this Batch b)
            => new(b.Id, b.DoseId, b.UserId, b.ExpiryDate, b.TotalQuantity, b.CountryOfOrigin, b.CookNumber, b.Notes);

        public static Batch ToDomain(this BatchCreateDto dto, int userId)
            => Batch.Create(dto.DoseId, userId, dto.ExpiryDate, dto.TotalQuantity, dto.CountryOfOrigin,
                dto.CookNumber ?? string.Empty, dto.Notes);
    }
}
