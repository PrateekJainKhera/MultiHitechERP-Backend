using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating dispatch challan
    /// </summary>
    public class CreateDispatchChallanRequest
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int QuantityDispatched { get; set; }

        [Required]
        [StringLength(500)]
        public string DeliveryAddress { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TransportMode { get; set; }

        [StringLength(50)]
        public string? VehicleNumber { get; set; }

        [StringLength(100)]
        public string? DriverName { get; set; }

        [StringLength(20)]
        public string? DriverContact { get; set; }

        [Range(1, int.MaxValue)]
        public int? NumberOfPackages { get; set; }

        [StringLength(50)]
        public string? PackagingType { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? TotalWeight { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [StringLength(100)]
        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
