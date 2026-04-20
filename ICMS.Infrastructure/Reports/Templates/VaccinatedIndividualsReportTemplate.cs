using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class VaccinatedIndividualsReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.VaccinatedIndividuals;

        public string Render(ReportData data)
        {
            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows);
            return ReportHtmlBase.Wrap(
                accentColor: "#0ea5e9",
                reportTitle: "Vaccinated Individuals Report",
                iconEmoji: "💉",
                data: data,
                bodyContent: table);
        }
    }
}
