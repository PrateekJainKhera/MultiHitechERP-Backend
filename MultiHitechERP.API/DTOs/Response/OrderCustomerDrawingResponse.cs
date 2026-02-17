namespace MultiHitechERP.API.DTOs.Response
{
    public class OrderCustomerDrawingResponse
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string DrawingType { get; set; } = "other";
        public string? Notes { get; set; }
        public long? FileSize { get; set; }
        public string? MimeType { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string? UploadedBy { get; set; }
    }
}
