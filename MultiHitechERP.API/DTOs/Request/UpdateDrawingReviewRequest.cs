using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating drawing review status (GATE)
    /// </summary>
    public class UpdateDrawingReviewRequest
    {
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Drawing review status is required")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reviewed by is required")]
        public string ReviewedBy { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}
