using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class InventoryReportFetcher : IReportDataFetcher
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryReportFetcher(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public ReportType ReportType => ReportType.Inventory;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime   = endDate.ToDateTime(TimeOnly.MaxValue);

            var batches = await _unitOfWork.BatchRepository
                .GetQueryable(false, ct, b => b.Dose, b => b.Transactions)
                .Where(b => b.Transactions.Any(t =>
                    t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime))
                .ToListAsync(ct);

            int totalIn  = batches.SelectMany(b => b.Transactions)
                .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                         && t.TransactionType == ICMS.Domain.Enums.TransactionType.In)
                .Sum(t => t.Quantity);

            int totalOut = batches.SelectMany(b => b.Transactions)
                .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                         && t.TransactionType == ICMS.Domain.Enums.TransactionType.Out)
                .Sum(t => t.Quantity);

            var rows = batches.Select(b => new ReportRow
            {
                Columns = new Dictionary<string, string?>
                {
                    ["Cook Number"]       = b.CookNumber,
                    ["Dose"]              = b.Dose?.DoseName ?? "-",
                    ["Country of Origin"] = b.CountryOfOrigin,
                    ["Expiry Date"]       = b.ExpiryDate.ToString("yyyy-MM-dd"),
                    ["Current Stock"]     = b.TotalQuantity.ToString(),
                    ["Total In (Period)"] = b.Transactions
                        .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                                 && t.TransactionType == ICMS.Domain.Enums.TransactionType.In)
                        .Sum(t => t.Quantity).ToString(),
                    ["Total Out (Period)"] = b.Transactions
                        .Where(t => t.TransactionDate >= startDateTime && t.TransactionDate <= endDateTime
                                 && t.TransactionType == ICMS.Domain.Enums.TransactionType.Out)
                        .Sum(t => t.Quantity).ToString(),
                }
            }).ToList();

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = rows.Count,
                SummaryStats  = new Dictionary<string, string>
                {
                    ["Total Stock In"]  = totalIn.ToString(),
                    ["Total Stock Out"] = totalOut.ToString(),
                    ["Net Movement"]    = (totalIn - totalOut).ToString()
                },
                ColumnHeaders = ["Cook Number", "Dose", "Country of Origin", "Expiry Date", "Current Stock", "Total In (Period)", "Total Out (Period)"],
                Rows          = rows
            };
        }
    }
}
