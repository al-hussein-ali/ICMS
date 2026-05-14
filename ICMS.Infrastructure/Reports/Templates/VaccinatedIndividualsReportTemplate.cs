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

            var accentColor = "#1e3a8a";
            var table = ReportHtmlBase.BuildDataTable(data.ColumnHeaders, data.Rows, tableTitle, accentColor, data.SummaryStats, isAr);
            
            return ReportHtmlBase.Wrap(
                accentColor: accentColor,
                reportTitle: reportTitle,
                iconEmoji: "💉",
                data: data,
                bodyContent: table);
        }
    }
}
