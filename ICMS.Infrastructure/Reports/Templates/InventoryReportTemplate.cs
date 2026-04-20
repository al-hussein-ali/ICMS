using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class InventoryReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.Inventory;

        public string Render(ReportData data)
        {
            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows);
            return ReportHtmlBase.Wrap(
                accentColor: "#10b981",
                reportTitle: "Inventory Report",
                iconEmoji: "📦",
                data: data,
                bodyContent: table);
        }
    }
}
