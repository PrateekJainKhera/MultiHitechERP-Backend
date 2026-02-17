namespace MultiHitechERP.API.Models.Orders
{
    public class OrderCustomerDrawing
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public string? MimeType { get; set; }
        public string DrawingType { get; set; } = "other";
        public string? Notes { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string? UploadedBy { get; set; }
    }
}
