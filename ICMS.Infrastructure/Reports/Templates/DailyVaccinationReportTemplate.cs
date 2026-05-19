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
            var tableTitle = isAr ? "سجلات المستفيدين المطعّمين" : "Vaccinated Beneficiary Records";
            var reportTitle = isAr ? "تقرير التطعيم اليومي" : "Daily Immunization Report";

            var accentColor = "#1e3a8a";

            // 1. Primary table: Vaccinated Individuals (without footer summary)
            body.Append(ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle, accentColor, null, isAr));

            // 2. Secondary table: Subtracted Inventory Transactions (if any exist)
            if (data.SecondaryColumnHeaders != null && data.SecondaryColumnHeaders.Count > 0 && data.SecondaryRows != null)
            {
                var secTitle = !string.IsNullOrEmpty(data.SecondaryTableTitle) 
                    ? data.SecondaryTableTitle 
                    : (isAr ? "الجرعات والتشغيلات المنصرفة من المخزون" : "Dose Batches Subtracted from Inventory");
                body.Append(ReportHtmlBase.BuildDataTable(data.SecondaryColumnHeaders, data.SecondaryRows, secTitle, accentColor, null, isAr));
            }

            // 3. Summary stats block at the bottom
            if (data.SummaryStats != null && data.SummaryStats.Count > 0)
            {
                body.Append(ReportHtmlBase.BuildSummaryStatsBlock(data.SummaryStats, accentColor, isAr));
            }

            return ReportHtmlBase.Wrap(
                accentColor: accentColor,
                reportTitle: reportTitle,
                iconEmoji: "📋",
                data: data,
                bodyContent: body.ToString());
        }
    }
}
