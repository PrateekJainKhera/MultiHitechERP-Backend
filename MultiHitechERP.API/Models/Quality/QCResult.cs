using System;

namespace MultiHitechERP.API.Models.Quality
{
    /// <summary>
    /// Represents quality control inspection results for a job card
    /// </summary>
    public class QCResult
    {
        public Guid Id { get; set; }
        public Guid JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public Guid? OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Inspection Details
        public string InspectionType { get; set; } = "In-Process";
        public DateTime InspectionDate { get; set; }
        public string? InspectedBy { get; set; }

        // Quantities
        public int QuantityInspected { get; set; }
        public int QuantityPassed { get; set; }
        public int QuantityRejected { get; set; }
        public int? QuantityRework { get; set; }

        // Status
        public string QCStatus { get; set; } = "Pending";

        // Defect Details
        public string? DefectDescription { get; set; }
        public string? DefectCategory { get; set; }
        public string? RejectionReason { get; set; }

        // Measurements (JSON string for dimensional checks)
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
