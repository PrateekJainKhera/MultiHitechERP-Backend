using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a new order
    /// Supports both single-product (backward compatible) and multi-product orders
    /// </summary>
    public class CreateOrderRequest
    {
        // OrderDate is optional - will default to current date if not provided
        public DateTime? OrderDate { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        // ===== MULTI-PRODUCT SUPPORT (NEW) =====
        /// <summary>
        /// List of order items (products). Use this for multi-product orders.
        /// If provided, the legacy single-product fields (ProductId, Quantity, etc.) are ignored.
        /// </summary>
        public List<CreateOrderItemRequest>? Items { get; set; }

        // ===== LEGACY SINGLE-PRODUCT FIELDS (Backward Compatible) =====
        /// <summary>
        /// Legacy: Due date for single-product orders. For multi-product, each item has its own due date.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Legacy: Product ID for single-product orders. For multi-product, use Items list.
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// Legacy: Quantity for single-product orders. For multi-product, use Items list.
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Legacy: Priority for single-product orders. For multi-product, each item has its own priority.
        /// </summary>
        public string? Priority { get; set; }

        // ===== ORDER-LEVEL FIELDS =====
        // Order Source & Agent
        public string OrderSource { get; set; } = "Direct";
        public int? AgentCustomerId { get; set; }
        public decimal? AgentCommission { get; set; }

        // Scheduling
        public string SchedulingStrategy { get; set; } = "Due Date";

        // Customer Requirements
        public string? CustomerMachine { get; set; }

        // Legacy: For backward compatibility with single-product orders
        public int? PrimaryDrawingId { get; set; }
        public string? DrawingSource { get; set; }
        public string? DrawingNotes { get; set; }
        public int? LinkedProductTemplateId { get; set; }
        public string? MaterialGradeRemark { get; set; }

        // Financial
        public decimal? OrderValue { get; set; }
        public decimal? AdvancePayment { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
    }
}
