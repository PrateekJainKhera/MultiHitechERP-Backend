using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for QC Result
    /// </summary>
    public class QCResultResponse
    {
        public Guid Id { get; set; }
        public Guid JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public Guid? OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Inspection Details
        public string InspectionType { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; }
        public string? InspectedBy { get; set; }

        // Quantities
        public int QuantityInspected { get; set; }
        public int QuantityPassed { get; set; }
        public int QuantityRejected { get; set; }
        public int? QuantityRework { get; set; }

        // Status
        public string QCStatus { get; set; } = string.Empty;

        // Defect Details
        public string? DefectDescription { get; set; }
        public string? DefectCategory { get; set; }
        public string? RejectionReason { get; set; }

        // Measurements
        public string? MeasurementData { get; set; }

        // Corrective Action
        public string? CorrectiveAction { get; set; }
        public bool RequiresRework { get; set; }

        // Approval
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
