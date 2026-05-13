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
            var reportTitle = isAr ? "تقرير المخزون" : "Inventory Report";

            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle);
            
            return ReportHtmlBase.Wrap(
                accentColor: "#10b981",
                reportTitle: reportTitle,
                iconEmoji: "📦",
                data: data,
                bodyContent: table);
        }
    }
}
