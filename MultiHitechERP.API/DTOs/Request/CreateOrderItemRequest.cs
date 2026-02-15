using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating an order item (product line in an order)
    /// </summary>
    public class CreateOrderItemRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";

        // Drawing Linkage (optional)
        public int? PrimaryDrawingId { get; set; }
        public string? DrawingSource { get; set; } // 'customer' or 'company'

        // Template Linkage (optional)
        public int? LinkedProductTemplateId { get; set; }

        // Material Remark (optional)
        public string? MaterialGradeRemark { get; set; }
    }
}
