using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class PregnantWomenReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.PregnantWomen;

        public string Render(ReportData data)
        {
            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows);
            return ReportHtmlBase.Wrap(
                accentColor: "#ec4899",
                reportTitle: "Pregnant Women Report",
                iconEmoji: "🤰",
                data: data,
                bodyContent: table);
        }
    }
}
