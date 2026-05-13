using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;
using System;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class PregnantWomenReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.PregnantWomen;

        public string Render(ReportData data)
        {
            var isAr = data.Lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            var tableTitle = isAr ? "بيانات الحوامل" : "Pregnant Women Data";
            var reportTitle = isAr ? "تقرير الحوامل" : "Pregnant Women Report";

            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle);
            
            return ReportHtmlBase.Wrap(
                accentColor: "#1e3a8a",
                reportTitle: reportTitle,
                iconEmoji: "🤱",
                data: data,
                bodyContent: table);
        }
    }
}
