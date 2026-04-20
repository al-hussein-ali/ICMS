using ICMS.Application.DTOs.Reports;
using System.Text;

namespace ICMS.Infrastructure.Reports.Templates
{
    /// <summary>
    /// Shared HTML rendering helper used by all report template renderers.
    /// Provides a professional, print-ready layout with ICMS branding.
    /// </summary>
    public static class ReportHtmlBase
    {
        public static string Wrap(string accentColor, string reportTitle, string iconEmoji, ReportData data, string bodyContent)
        {
            var summaryHtml = BuildSummaryCards(data.SummaryStats);
            var statsBar = $@"
                <div class='stats-bar'>
                    <div class='stat-item'><span class='stat-label'>Report Period</span><span class='stat-value'>{data.StartDate:yyyy-MM-dd} → {data.EndDate:yyyy-MM-dd}</span></div>
                    <div class='stat-item'><span class='stat-label'>Total Records</span><span class='stat-value'>{data.TotalRecords}</span></div>
                    <div class='stat-item'><span class='stat-label'>Generated At</span><span class='stat-value'>{data.GeneratedAt}</span></div>
                </div>";

            return $@"<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<title>{reportTitle} — ICMS Report</title>
<style>
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

  * {{ box-sizing: border-box; margin: 0; padding: 0; }}
  body {{ font-family: 'Inter', sans-serif; background: #f0f4f8; color: #1e2a3b; font-size: 12px; }}

  .page {{ max-width: 1100px; margin: 0 auto; padding: 24px; }}

  /* ── Header ── */
  .header {{ background: linear-gradient(135deg, {accentColor} 0%, {Darken(accentColor)} 100%);
             border-radius: 16px; padding: 32px 40px; color: #fff; margin-bottom: 24px;
             display: flex; align-items: center; gap: 24px; }}
  .header-icon {{ font-size: 48px; }}
  .header-meta h1 {{ font-size: 26px; font-weight: 700; letter-spacing: -0.5px; }}
  .header-meta p  {{ font-size: 13px; opacity: 0.85; margin-top: 4px; }}
  .header-logo {{ margin-left: auto; font-size: 13px; opacity: 0.75; text-align: right; line-height: 1.6; }}

  /* ── Stats bar ── */
  .stats-bar {{ display: flex; gap: 16px; margin-bottom: 20px; }}
  .stat-item {{ background: #fff; border-radius: 10px; padding: 14px 20px; flex: 1;
               box-shadow: 0 1px 4px rgba(0,0,0,.08); border-left: 4px solid {accentColor}; }}
  .stat-label {{ display: block; font-size: 10px; color: #6b7c93; font-weight: 600;
                text-transform: uppercase; letter-spacing: .5px; margin-bottom: 4px; }}
  .stat-value {{ font-size: 14px; font-weight: 700; color: #1e2a3b; }}

  /* ── Summary cards ── */
  .summary-section {{ margin-bottom: 24px; }}
  .summary-title {{ font-size: 11px; font-weight: 700; color: #6b7c93; text-transform: uppercase;
                   letter-spacing: .8px; margin-bottom: 12px; }}
  .summary-cards {{ display: flex; gap: 12px; flex-wrap: wrap; }}
  .summary-card {{ background: #fff; border-radius: 10px; padding: 16px 20px; min-width: 140px;
                  box-shadow: 0 1px 4px rgba(0,0,0,.08); text-align: center; }}
  .summary-card .card-val {{ font-size: 22px; font-weight: 700; color: {accentColor}; }}
  .summary-card .card-key {{ font-size: 10px; color: #6b7c93; font-weight: 600;
                            text-transform: uppercase; margin-top: 4px; }}

  /* ── Table ── */
  .table-wrapper {{ background: #fff; border-radius: 12px; box-shadow: 0 1px 6px rgba(0,0,0,.08);
                   overflow: hidden; margin-bottom: 24px; }}
  .table-header {{ padding: 16px 20px; border-bottom: 1px solid #e8edf3;
                  font-size: 11px; font-weight: 700; color: #6b7c93; text-transform: uppercase;
                  letter-spacing: .8px; background: #f8fafc; }}
  table {{ width: 100%; border-collapse: collapse; }}
  thead tr {{ background: {accentColor}; color: #fff; }}
  thead th {{ padding: 10px 14px; text-align: left; font-size: 10px; font-weight: 600;
             text-transform: uppercase; letter-spacing: .5px; white-space: nowrap; }}
  tbody tr {{ border-bottom: 1px solid #f0f4f8; transition: background .15s; }}
  tbody tr:nth-child(even) {{ background: #f8fafc; }}
  tbody tr:last-child {{ border-bottom: none; }}
  tbody td {{ padding: 9px 14px; font-size: 11px; color: #374151; }}

  /* ── Footer ── */
  .footer {{ text-align: center; color: #9ca3af; font-size: 10px; padding-top: 12px;
            border-top: 1px solid #e5e7eb; }}

  @media print {{
    body {{ background: #fff; }}
    .page {{ padding: 0; }}
  }}
</style>
</head>
<body>
<div class='page'>

  <div class='header'>
    <div class='header-icon'>{iconEmoji}</div>
    <div class='header-meta'>
      <h1>{reportTitle}</h1>
      <p>Immunization & Community Management System (ICMS)</p>
    </div>
    <div class='header-logo'>
      🏥 ICMS<br>Official Report<br>{data.GeneratedAt}
    </div>
  </div>

  {statsBar}

  {summaryHtml}

  {bodyContent}

  <div class='footer'>
    This report was automatically generated by ICMS · {data.GeneratedAt} · Confidential
  </div>

</div>
</body>
</html>";
        }

        private static string BuildSummaryCards(Dictionary<string, string> stats)
        {
            if (stats.Count == 0) return string.Empty;

            var cards = new StringBuilder();
            foreach (var kv in stats)
                cards.Append($"<div class='summary-card'><div class='card-val'>{kv.Value}</div><div class='card-key'>{kv.Key}</div></div>");

            return $@"<div class='summary-section'>
              <div class='summary-title'>Summary Statistics</div>
              <div class='summary-cards'>{cards}</div>
            </div>";
        }

        public static string BuildDataTable(List<string> headers, List<ReportRow> rows)
        {
            var thead = new StringBuilder();
            foreach (var h in headers) thead.Append($"<th>{h}</th>");

            var tbody = new StringBuilder();
            if (rows.Count == 0)
            {
                tbody.Append($"<tr><td colspan='{headers.Count}' style='text-align:center;padding:24px;color:#9ca3af;'>No records found for this period.</td></tr>");
            }
            else
            {
                foreach (var row in rows)
                {
                    tbody.Append("<tr>");
                    foreach (var h in headers)
                        tbody.Append($"<td>{row.Columns.GetValueOrDefault(h) ?? "-"}</td>");
                    tbody.Append("</tr>");
                }
            }

            return $@"<div class='table-wrapper'>
              <div class='table-header'>Report Data ({rows.Count} records)</div>
              <table><thead><tr>{thead}</tr></thead><tbody>{tbody}</tbody></table>
            </div>";
        }

        /// <summary>Simple darkening: reduces hex by ~20%.</summary>
        private static string Darken(string hex)
        {
            if (hex.StartsWith('#') && hex.Length == 7)
            {
                try
                {
                    int r = Math.Max(0, Convert.ToInt32(hex[1..3], 16) - 40);
                    int g = Math.Max(0, Convert.ToInt32(hex[3..5], 16) - 40);
                    int b = Math.Max(0, Convert.ToInt32(hex[5..7], 16) - 40);
                    return $"#{r:X2}{g:X2}{b:X2}";
                }
                catch
                {
                    // Fallback to original color if hex processing fails
                }
            }
            return hex;
        }
    }
}
