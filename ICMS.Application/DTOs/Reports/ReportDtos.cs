using ICMS.Application.Enums;
using System;
using System.Collections.Generic;

namespace ICMS.Application.DTOs.Reports
{
    public class ReportRequestDto
    {
        public ReportType ReportType { get; set; }
        public ReportFormat Format { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Lang { get; set; } = "en";
        public Dictionary<string, string>? AdditionalParameters { get; set; } = new();
    }

    public record ReportJobResponseDto(string JobId, string Message);
    public record ReportStatusDto(string JobId, string Status, string? DownloadUrl, string? ErrorMessage);

    // Generic container for report tabular data
    public class ReportRow
    {
        public Dictionary<string, string?> Columns { get; set; } = new();
    }

    public class ReportData
    {
        public ReportType ReportType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string GeneratedAt { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public string Lang { get; set; } = "en";
        public Dictionary<string, string> SummaryStats { get; set; } = new();
        public List<string> ColumnHeaders { get; set; } = new();
        public List<ReportRow> Rows { get; set; } = new();
    }
}
