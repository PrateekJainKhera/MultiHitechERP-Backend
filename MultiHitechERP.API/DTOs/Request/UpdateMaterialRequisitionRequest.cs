using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating an existing material requisition
    /// </summary>
    public class UpdateMaterialRequisitionRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RequisitionNo { get; set; } = string.Empty;

        [Required]
        public DateTime RequisitionDate { get; set; }

        public int? JobCardId { get; set; }

        [StringLength(50)]
        public string? JobCardNo { get; set; }

        public int? OrderId { get; set; }

        [StringLength(50)]
        public string? OrderNo { get; set; }

        public int? OrderItemId { get; set; } // For multi-product orders

        [StringLength(10)]
        public string? ItemSequence { get; set; } // A, B, C, D...

        [StringLength(200)]
        public string? CustomerName { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueDate { get; set; }

        [StringLength(100)]
        public string? RequestedBy { get; set; }

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
