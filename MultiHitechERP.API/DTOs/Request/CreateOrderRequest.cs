using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a new order
    /// </summary>
    public class CreateOrderRequest
    {
        // OrderDate is optional - will default to current date if not provided
        public DateTime? OrderDate { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";

        // Order Source & Agent
        [Required(ErrorMessage = "Order source is required")]
        public string OrderSource { get; set; } = "Direct";

        public int? AgentCustomerId { get; set; }
        public decimal? AgentCommission { get; set; }

        // Scheduling
        [Required(ErrorMessage = "Scheduling strategy is required")]
        public string SchedulingStrategy { get; set; } = "Due Date";

        // Drawing Linkage
        public int? PrimaryDrawingId { get; set; }
        public string? DrawingSource { get; set; } // 'customer' or 'company'
        public string? DrawingNotes { get; set; }

        // Template Linkage
        public int? LinkedProductTemplateId { get; set; }

        // Customer Requirements
        public string? CustomerMachine { get; set; }
        public string? MaterialGradeRemark { get; set; }

        // Financial
        public decimal? OrderValue { get; set; }
        public decimal? AdvancePayment { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
    }
}
