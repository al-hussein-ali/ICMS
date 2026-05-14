using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;
using System.Text;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class DailyVaccinationReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.DailyVaccination;

        public string Render(ReportData data)
        {
            var body = new StringBuilder();
            var isAr = data.Lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            var tableTitle = isAr ? "بيانات التقرير" : "Report Data";
            var reportTitle = isAr ? "إحصائيات التطعيم اليومية" : "Daily Vaccination Statistics";

            var accentColor = "#1e3a8a";
            body.Append(ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle, accentColor, data.SummaryStats, isAr));

            return ReportHtmlBase.Wrap(
                accentColor: accentColor,
                reportTitle: reportTitle,
                iconEmoji: "📋",
                data: data,
                bodyContent: body.ToString());
        }
    }
}
