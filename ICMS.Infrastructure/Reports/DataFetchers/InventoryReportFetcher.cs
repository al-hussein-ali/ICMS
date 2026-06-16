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

            var safeParams = parameters != null 
                ? new Dictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase) 
                : new Dictionary<string, string>();

            // ── Parse filters & Build Subtitle ────────────────────────────────
            var pills = new List<string>();

            var transactionType = (TransactionType?)null;
            if (safeParams.TryGetValue("transactionType", out var typeStr) && !string.IsNullOrEmpty(typeStr))
            {
                if (typeStr.Equals("Incoming", StringComparison.OrdinalIgnoreCase))
                {
                    transactionType = TransactionType.In;
                    pills.Add($"<span class='filter-pill'>{(isAr ? "نوع الحركة: وارد" : "Transaction Type: Incoming")}</span>");
                }
                else if (typeStr.Equals("Outgoing", StringComparison.OrdinalIgnoreCase))
                {
                    transactionType = TransactionType.Out;
                    pills.Add($"<span class='filter-pill'>{(isAr ? "نوع الحركة: صادر" : "Transaction Type: Outgoing")}</span>");
                }
            }

            var vaccineId = (int?)null;
            if (safeParams.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
            {
                vaccineId = vId;
                var vaccine = await unitOfWork.VaccineRepository.GetByIdAsync(vId, ct);
                if (vaccine != null)
                {
                    var vName = ICMS.Application.Extensions.LocalizationHelper.GetLocalizedValue(vaccine.VaccineName, lang);
                    pills.Add($"<span class='filter-pill'>{(isAr ? "اللقاح" : "Vaccine")}: {vName}</span>");
                }
            }

            // batchStatus filter: "Active" = not expired as of today, "Expired" = past expiry date
            bool? isExpiredFilter = null;
            if (safeParams.TryGetValue("batchStatus", out var bsStr) && !string.IsNullOrEmpty(bsStr))
            {
                if (bsStr.Equals("Expired", StringComparison.OrdinalIgnoreCase))
                {
                    isExpiredFilter = true;
                    pills.Add($"<span class='filter-pill'>{(isAr ? "حالة الدفعة: منتهية الصلاحية" : "Batch Status: Expired")}</span>");
                }
                else if (bsStr.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    isExpiredFilter = false;
                    pills.Add($"<span class='filter-pill'>{(isAr ? "حالة الدفعة: نشطة" : "Batch Status: Active")}</span>");
                }
            }

            int? doseIdFilter = null;
            if (safeParams.TryGetValue("doseId", out var dIdStr) && int.TryParse(dIdStr, out var dId))
            {
                doseIdFilter = dId;
                var dose = await unitOfWork.DoseRepository.GetByIdAsync(dId, ct);
                if (dose != null)
                {
                    var dName = ICMS.Application.Extensions.LocalizationHelper.GetLocalizedValue(dose.DoseName, lang);
                    pills.Add($"<span class='filter-pill'>{(isAr ? "الجرعة" : "Dose")}: {dName}</span>");
                }
            }

            if (pills.Count == 0)
            {
                pills.Add($"<span class='filter-pill'>{(isAr ? "جميع السجلات" : "All Records")}</span>");
            }

            var subtitle = string.Join(" ", pills);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Retrieve period parameter
            string? period = null;
            safeParams.TryGetValue("period", out period);

            string periodPrefix = "";
            if (!string.IsNullOrEmpty(period))
            {
                periodPrefix = isAr
                    ? (period == "daily" ? "اليومي" : period == "weekly" ? "الأسبوعي" : period == "monthly" ? "الشهري" : "السنوي")
                    : (period == "daily" ? "Daily" : period == "weekly" ? "Weekly" : period == "monthly" ? "Monthly" : "Yearly");
            }

            string reportTitle;
            if (isAr)
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix)
                    ? $"تقرير مخزون اللقاحات ({startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd})"
                    : $"تقرير مخزون اللقاحات {periodPrefix.Trim()} ({startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd})";
            }
            else
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix)
                    ? $"Vaccine Inventory Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})"
                    : $"{periodPrefix} Vaccine Inventory Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})";
            }

            // ── Fetch data ────────────────────────────────────────────────────
            var batches = await queryable
                .Where(b => 
                    ((transactionType.HasValue && b.Transactions.Any(t => t.TransactionType == transactionType.Value && t.TransactionDate >= startDateTime && t.TransactionDate < endDateTime)) ||
                     (!transactionType.HasValue && b.CreationDate <= endDate))
                    && (!vaccineId.HasValue || b.Dose.VaccineId == vaccineId.Value)
                    && (!doseIdFilter.HasValue || b.DoseId == doseIdFilter.Value)
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
                Subtitle      = subtitle,
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
