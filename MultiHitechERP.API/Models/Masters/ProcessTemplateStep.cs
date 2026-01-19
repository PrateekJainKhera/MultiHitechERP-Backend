using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents individual process steps in a process template
    /// </summary>
    public class ProcessTemplateStep
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public int StepNo { get; set; }

        // Process Reference
        public Guid ProcessId { get; set; }
        public string? ProcessCode { get; set; }
        public string? ProcessName { get; set; }

        // Machine Requirement
        public Guid? DefaultMachineId { get; set; }
        public string? DefaultMachineName { get; set; }
        public string? MachineType { get; set; }

        // Time Standards
        public int? SetupTimeMin { get; set; }
        public int? CycleTimeMin { get; set; }
        public decimal? CycleTimePerPiece { get; set; }

        // Sequence
        public bool IsParallel { get; set; }
        public int? ParallelGroupNo { get; set; }

        // Dependencies (JSON array of step numbers)
        public string? DependsOnSteps { get; set; }

        // Quality
        public bool RequiresQC { get; set; }
        public string? QCCheckpoints { get; set; }

        // Instructions
        public string? WorkInstructions { get; set; }
        public string? SafetyInstructions { get; set; }

        // Tooling
        public string? ToolingRequired { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
