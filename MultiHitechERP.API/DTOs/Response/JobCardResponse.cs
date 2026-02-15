using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class JobCardResponse
    {
        public int Id { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public string CreationType { get; set; } = "Auto-Generated";

        // Order Reference
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Order Item Reference (for multi-product orders)
        public int? OrderItemId { get; set; }
        public string? ItemSequence { get; set; }  // A, B, C, D...

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

        // Instructions
        public string? WorkInstructions { get; set; }
        public string? QualityCheckpoints { get; set; }
        public string? SpecialNotes { get; set; }

        // Quantity (target)
        public int Quantity { get; set; }

        // Status (Planning: Pending | Planned | Released)
        public string Status { get; set; } = "Pending";

        // Priority
        public string Priority { get; set; } = "Medium";

        // Manufacturing Dimensions
        public string? ManufacturingDimensions { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int Version { get; set; }

        // Material Requirements (optional - loaded when needed)
        public List<JobCardMaterialRequirementResponse>? MaterialRequirements { get; set; }
    }
}
