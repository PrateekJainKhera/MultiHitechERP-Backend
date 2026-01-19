using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating an existing order
    /// </summary>
    public class UpdateOrderRequest
    {
        [Required(ErrorMessage = "Order ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        public DateTime? AdjustedDueDate { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";

        public decimal? OrderValue { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? BalancePayment { get; set; }

        public string? DelayReason { get; set; }

        public string? UpdatedBy { get; set; }

        [Required(ErrorMessage = "Version is required for optimistic locking")]
        public int Version { get; set; }
    }
}
