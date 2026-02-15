using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateJobCardRequest
    {
        [Required(ErrorMessage = "Job card number is required")]
        public string JobCardNo { get; set; } = string.Empty;

        public string CreationType { get; set; } = "Manual";

        // Order Reference
        [Required(ErrorMessage = "Order ID is required")]
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
        [Required(ErrorMessage = "Process ID is required")]
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessCode { get; set; }
        public int? StepNo { get; set; }
        public int? ProcessTemplateId { get; set; }

        // Instructions (from process template)
        public string? WorkInstructions { get; set; }
        public string? QualityCheckpoints { get; set; }
        public string? SpecialNotes { get; set; }

        // Quantity
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Priority
        public string Priority { get; set; } = "Medium";

        // Manufacturing Dimensions (JSON string)
        public string? ManufacturingDimensions { get; set; }

        public string? CreatedBy { get; set; }

        // Dependencies (list of prerequisite job card IDs)
        public List<int>? PrerequisiteJobCardIds { get; set; }

        // Material Requirements (confirmed by planner during creation)
        public List<JobCardMaterialRequirementRequest>? MaterialRequirements { get; set; }
    }
}
