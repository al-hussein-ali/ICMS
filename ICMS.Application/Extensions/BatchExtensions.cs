using ICMS.Application.DTOs.Batch;
using ICMS.Domain.Entites.Clinical;
using System;

namespace ICMS.Application.Extensions
{
    public static class BatchExtensions
    {
        public static BatchReadDto ToReadDto(this Batch b)
        {
            var status = GetStatus(b);
            var remaining = status == "expired" ? 0 : b.TotalQuantity;
            
            return new BatchReadDto(
                b.Id, 
                b.BatchName, 
                b.DoseId, 
                LocalizationHelper.GetLocalizedValue(b.Dose?.DoseName) ?? "Unknown",
                LocalizationHelper.GetLocalizedValue(b.Dose?.Vaccine?.VaccineName) ?? "Unknown",
                b.UserId, 
                b.CreationDate, 
                b.ExpiryDate, 
                b.TotalQuantity + b.ConsumedQuantity, // Original Total
                remaining, // Remaining
                b.ConsumedQuantity,
                b.CookNumber,
                b.CountryOfOrigin,
                status
            );
        }

        public static BatchDetailsDto ToDetailsDto(this Batch b)
        {
            var status = GetStatus(b);
            var remaining = status == "expired" ? 0 : b.TotalQuantity;

            return new BatchDetailsDto(
                b.Id, 
                b.BatchName, 
                b.DoseId, 
                LocalizationHelper.GetLocalizedValue(b.Dose?.DoseName) ?? "Unknown",
                LocalizationHelper.GetLocalizedValue(b.Dose?.Vaccine?.VaccineName) ?? "Unknown",
                b.UserId, 
                b.CreationDate, 
                b.ExpiryDate, 
                b.TotalQuantity + b.ConsumedQuantity, 
                remaining, 
                b.ConsumedQuantity,
                b.CountryOfOrigin, 
                b.CookNumber, 
                b.Notes,
                status
            );
        }

        public static Batch ToDomain(this BatchCreateDto dto, int userId)
            => Batch.Create(dto.DoseId, userId, dto.BatchName, dto.CreationDate, dto.ExpiryDate, dto.TotalQuantity, dto.CountryOfOrigin,
                dto.CookNumber ?? string.Empty, dto.Notes);

        private static string GetStatus(Batch b)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (b.ExpiryDate < today) return "expired";
            if (b.ExpiryDate < today.AddMonths(3)) return "expiring";
            return "active";
        }
    }
}
