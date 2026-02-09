using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request to receive purchased components directly into inventory
    /// </summary>
    public class ReceiveComponentRequest
    {
        [Required(ErrorMessage = "Component ID is required")]
        public int ComponentId { get; set; }

        [Required(ErrorMessage = "Component name is required")]
        public string ComponentName { get; set; } = string.Empty;

        public string? PartNumber { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; } = "PCS";

        public decimal? UnitCost { get; set; }

        public int? SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? PONo { get; set; }

        public DateTime? PODate { get; set; }

        [Required(ErrorMessage = "Receipt date is required")]
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        public string? StorageLocation { get; set; } = "Main Warehouse";

        public string? Remarks { get; set; }

        [Required(ErrorMessage = "Received by is required")]
        public string ReceivedBy { get; set; } = string.Empty;
    }
}
