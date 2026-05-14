using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class InventoryReportFetcher(IUnitOfWork unitOfWork) : IReportDataFetcher
    {
        public ReportType ReportType => ReportType.Inventory;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", Dictionary<string, string>? parameters = null, CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            var startDateTime = DateTime.SpecifyKind(startDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var endDateTime   = DateTime.SpecifyKind(endDate.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);

            var queryable = unitOfWork.BatchRepository
                .GetQueryable(false, ct, b => b.Dose, b => b.Transactions);

            if (queryable == null)
                throw new InvalidOperationException("Failed to get queryable from repository.");

            var transactionType = (TransactionType?)null;
            if (parameters != null && parameters.TryGetValue("transactionType", out var typeStr) && !string.IsNullOrEmpty(typeStr))
            {
                if (Enum.TryParse<TransactionType>(typeStr, true, out var tType))
                    transactionType = tType;
            }

            var vaccineId = (int?)null;
            if (parameters != null && parameters.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
                vaccineId = vId;

            var batches = await queryable
                .Where(b => b.Transactions.Any(t =>
                    t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime &&
                    (!transactionType.HasValue || t.TransactionType == transactionType.Value))
                    && (!vaccineId.HasValue || b.Dose.VaccineId == vaccineId.Value))
                .ToListAsync(ct);

            // ── Labels ───────────────────────────────────────────────────
            var colCook      = isAr ? "رقم الطبخة"           : "Cook Number";
            var colDose      = isAr ? "الجرعة"               : "Dose";
            var colOrigin    = isAr ? "بلد المنشأ"           : "Country of Origin";
            var colExpiry    = isAr ? "تاريخ الانتهاء"       : "Expiry Date";
            var colStock     = isAr ? "المخزون الحالي"       : "Current Stock";
            var colTotalIn   = isAr ? "إجمالي الوارد (الفترة)" : "Total In (Period)";
            var colTotalOut  = isAr ? "إجمالي الصادر (الفترة)" : "Total Out (Period)";

            var lblStockIn   = isAr ? "إجمالي الوارد"        : "Total Stock In";
            var lblStockOut  = isAr ? "إجمالي الصادر"        : "Total Stock Out";
            var lblNetMove   = isAr ? "صافي الحركة"          : "Net Movement";

            int totalIn  = batches.SelectMany(b => b.Transactions)
                .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                         && t.TransactionType == TransactionType.In)
                .Sum(t => t.Quantity);

            int totalOut = batches.SelectMany(b => b.Transactions)
                .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                         && t.TransactionType == TransactionType.Out)
                .Sum(t => t.Quantity);

            var rows = batches.Select(b => new ReportRow
            {
                Columns = new Dictionary<string, string?>
                {
                    [colCook]      = b.CookNumber,
                    [colDose]      = b.Dose?.DoseName ?? "-",
                    [colOrigin]    = b.CountryOfOrigin,
                    [colExpiry]    = b.ExpiryDate.ToString("yyyy-MM-dd"),
                    [colStock]     = b.TotalQuantity.ToString(),
                    [colTotalIn]   = b.Transactions
                        .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                                 && t.TransactionType == TransactionType.In)
                        .Sum(t => t.Quantity).ToString(),
                    [colTotalOut]  = b.Transactions
                        .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                                 && t.TransactionType == TransactionType.Out)
                        .Sum(t => t.Quantity).ToString(),
                }
            }).ToList();

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                Lang          = lang,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = rows.Count,
                SummaryStats  = new Dictionary<string, string>
                {
                    [lblStockIn]  = totalIn.ToString(),
                    [lblStockOut] = totalOut.ToString(),
                    [lblNetMove]  = (totalIn - totalOut).ToString()
                },
                ColumnHeaders = [colCook, colDose, colOrigin, colExpiry, colStock, colTotalIn, colTotalOut],
                Rows          = rows
            };
        }
    }
}
