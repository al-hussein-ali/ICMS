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
            var endDateTime   = DateTime.SpecifyKind(endDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc).AddDays(1);

            var queryable = unitOfWork.BatchRepository
                .GetQueryable(false, ct, b => b.Dose, b => b.Transactions);

            if (queryable == null)
                throw new InvalidOperationException("Failed to get queryable from repository.");

            // ── Parse filters ─────────────────────────────────────────────────
            var transactionType = (TransactionType?)null;
            if (parameters != null && parameters.TryGetValue("transactionType", out var typeStr) && !string.IsNullOrEmpty(typeStr))
            {
                if (Enum.TryParse<TransactionType>(typeStr, true, out var tType))
                    transactionType = tType;
            }

            var vaccineId = (int?)null;
            if (parameters != null && parameters.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
                vaccineId = vId;

            // batchStatus filter: "Active" = not expired as of today, "Expired" = past expiry date
            bool? isExpiredFilter = null;
            if (parameters != null && parameters.TryGetValue("batchStatus", out var bsStr) && !string.IsNullOrEmpty(bsStr))
            {
                if (bsStr.Equals("Expired", StringComparison.OrdinalIgnoreCase))
                    isExpiredFilter = true;
                else if (bsStr.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    isExpiredFilter = false;
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Retrieve period parameter
            string? period = null;
            if (parameters != null && parameters.TryGetValue("period", out var pStr))
            {
                period = pStr;
            }

            string periodPrefix = "";
            if (!string.IsNullOrEmpty(period))
            {
                periodPrefix = isAr
                    ? (period == "daily" ? "اليومي" : period == "weekly" ? "الأسبوعي" : period == "monthly" ? "الشهري" : "السنوي")
                    : (period == "daily" ? "Daily" : period == "weekly" ? "Weekly" : period == "monthly" ? "Monthly" : "Yearly");
            }

            // ── Dynamic title ─────────────────────────────────────────────────
            var titleParts = new List<string>();
            if (transactionType.HasValue)
                titleParts.Add(isAr
                    ? (transactionType.Value == TransactionType.In ? "وارد" : transactionType.Value == TransactionType.Out ? "صادر" : "تسوية")
                    : (transactionType.Value == TransactionType.In ? "Incoming" : transactionType.Value == TransactionType.Out ? "Outgoing" : "Adjustment"));
            if (isExpiredFilter.HasValue)
                titleParts.Add(isAr
                    ? (isExpiredFilter.Value ? "منتهية الصلاحية" : "نشطة")
                    : (isExpiredFilter.Value ? "Expired Batches" : "Active Batches"));

            string reportTitle;
            string baseTitle = titleParts.Count > 0
                ? string.Join(" — ", titleParts) + (isAr ? " — تقرير المخزون" : " — Inventory Report")
                : (isAr ? "تقرير المخزون" : "Inventory Report");

            if (isAr)
            {
                if (baseTitle == "تقرير المخزون")
                    reportTitle = $"تقرير المخزون {periodPrefix}";
                else
                    reportTitle = $"{baseTitle} {periodPrefix}";
            }
            else
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix) ? baseTitle : $"{periodPrefix} {baseTitle}";
            }

            // ── Fetch data ────────────────────────────────────────────────────
            var batches = await queryable
                .Where(b => b.Transactions.Any(t =>
                    t.TransactionDate >= startDateTime && t.TransactionDate < endDateTime &&
                    (!transactionType.HasValue || t.TransactionType == transactionType.Value))
                    && (!vaccineId.HasValue || b.Dose.VaccineId == vaccineId.Value)
                    && (!isExpiredFilter.HasValue ||
                        (isExpiredFilter.Value ? b.ExpiryDate < today : b.ExpiryDate >= today)))
                .ToListAsync(ct);

            // ── Labels ────────────────────────────────────────────────────────
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
                    [colDose]      = ICMS.Application.Extensions.LocalizationHelper.GetLocalizedValue(b.Dose?.DoseName, lang) ?? "-",
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
                ReportTitle   = reportTitle,
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
