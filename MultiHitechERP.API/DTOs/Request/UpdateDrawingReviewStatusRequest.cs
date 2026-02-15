namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateDrawingReviewStatusRequest
    {
        public string DrawingReviewStatus { get; set; } = string.Empty;
        public string? DrawingReviewNotes { get; set; }
        public string DrawingReviewedBy { get; set; } = string.Empty;
    }
}
