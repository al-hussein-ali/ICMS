using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;
using System;

namespace ICMS.Infrastructure.Reports.Templates
{
    public class VaccinatedIndividualsReportTemplate : IReportTemplateRenderer
    {
        public ReportType ReportType => ReportType.VaccinatedIndividuals;

        public string Render(ReportData data)
        {
            var isAr = data.Lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            var tableTitle = isAr ? "سجل الأفراد" : "Individuals Record";
            var reportTitle = isAr ? "تقرير الأفراد الملقحين" : "Vaccinated Individuals Report";

            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle);
            
            return ReportHtmlBase.Wrap(
                accentColor: "#1e3a8a",
                reportTitle: reportTitle,
                iconEmoji: "💉",
                data: data,
                bodyContent: table);
        }
    }
}
