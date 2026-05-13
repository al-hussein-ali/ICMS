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

            body.Append(ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle));

            return ReportHtmlBase.Wrap(
                accentColor: "#1e3a8a",
                reportTitle: reportTitle,
                iconEmoji: "📋",
                data: data,
                bodyContent: body.ToString());
        }
    }
}
