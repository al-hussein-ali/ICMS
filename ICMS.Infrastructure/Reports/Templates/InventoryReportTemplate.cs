using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;
using System;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class InventoryReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.Inventory;

        public string Render(ReportData data)
        {
            var isAr = data.Lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            var tableTitle = isAr ? "بيانات المخزون" : "Inventory Data";
            var reportTitle = data.ReportTitle ?? (isAr ? "تقرير المخزون" : "Inventory Report");

            var accentColor = "#0f766e"; // teal — matches Inventory UI card color
            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle, accentColor, data.SummaryStats, isAr);
            
            return ReportHtmlBase.Wrap(
                accentColor: accentColor,
                reportTitle: reportTitle,
                iconEmoji: "📦",
                data: data,
                bodyContent: table);
        }
    }
}
