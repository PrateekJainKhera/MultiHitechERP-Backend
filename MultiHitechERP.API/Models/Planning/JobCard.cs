using System;

namespace MultiHitechERP.API.Models.Planning
{
    /// <summary>
    /// Represents a job card for planning — core manufacturing operation record.
    /// Scheduling and Production fields live in their own tables.
    /// </summary>
    public class JobCard
    {
        // Identity
        public int Id { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public string CreationType { get; set; } = "Auto-Generated";

        // Order Reference
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Drawing
        public int? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }
        public string? DrawingName { get; set; }
        public string DrawingSelectionType { get; set; } = "auto";

        // Child Part
        public int? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }
        public int? ChildPartTemplateId { get; set; }

        // Process
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessCode { get; set; }
        public int? StepNo { get; set; }
        public int? ProcessTemplateId { get; set; }

        // Instructions (from process template)
        public string? WorkInstructions { get; set; }
        public string? QualityCheckpoints { get; set; }
        public string? SpecialNotes { get; set; }

        // Quantity (target — execution tracked in Production)
        public int Quantity { get; set; }

        // Status (Planning lifecycle: Pending → Planned → Released)
        public string Status { get; set; } = "Pending";

        // Priority
        public string Priority { get; set; } = "Medium";

        // Manufacturing Dimensions (JSON from drawing)
        public string? ManufacturingDimensions { get; set; }

        // Production Execution (shop-floor tracking)
        public string ProductionStatus { get; set; } = "Pending";
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int CompletedQty { get; set; }
        public int RejectedQty { get; set; }
        public bool ReadyForAssembly { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int Version { get; set; } = 1;
    }
}
