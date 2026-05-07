namespace ICMS.Application.DTOs.BulkResult
{
    public class BulkSyncResultDto
    {
        public int SuccessCount { get; set; }
        public List<SyncSuccessDetail> Successes { get; set; } = new();
        public List<SyncFailureDetail> Failures { get; set; } = new();
    }

    public record SyncSuccessDetail(string? CorrelationId, int IndividualId);
    public record SyncFailureDetail(string? CorrelationId, string FullName, string Error);
}
