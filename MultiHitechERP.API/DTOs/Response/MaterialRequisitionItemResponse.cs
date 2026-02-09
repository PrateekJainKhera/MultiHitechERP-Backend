using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for material requisition item data
    /// </summary>
    public class MaterialRequisitionItemResponse
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int LineNo { get; set; }

        public int MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialGrade { get; set; }

        public decimal QuantityRequired { get; set; }
        public string? UOM { get; set; }
        public decimal? LengthRequiredMM { get; set; }
        public decimal? DiameterMM { get; set; }
        public int? NumberOfPieces { get; set; }

        public decimal? QuantityAllocated { get; set; }
        public decimal? QuantityIssued { get; set; }
        public decimal? QuantityPending { get; set; }

        public string Status { get; set; } = string.Empty;

        public int? JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public int? ProcessId { get; set; }
        public string? ProcessName { get; set; }

        public string? Remarks { get; set; }
        public DateTime? AllocatedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
