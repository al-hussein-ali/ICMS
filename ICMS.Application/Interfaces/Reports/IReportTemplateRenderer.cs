using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;

namespace ICMS.Application.Interfaces.Reports
{
    /// <summary>
    /// Strategy interface — each report type provides its own HTML template renderer.
    /// Renders a ReportData object into a styled HTML string suitable for PDF conversion.
    /// </summary>
    public interface IReportTemplateRenderer
    {
        ReportType ReportType { get; }
        string Render(ReportData data);
    }
}
