using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateDrawingRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Drawing name is required")]
        [StringLength(200, ErrorMessage = "Drawing name cannot exceed 200 characters")]
        public string DrawingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Drawing type is required")]
        [StringLength(100, ErrorMessage = "Drawing type cannot exceed 100 characters")]
        public string DrawingType { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Revision number cannot exceed 20 characters")]
        public string? RevisionNumber { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
