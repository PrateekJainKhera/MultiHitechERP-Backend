using System;

namespace MultiHitechERP.API.Models.Planning
{
    /// <summary>
    /// Represents dependencies between job cards (many-to-many relationship)
    /// </summary>
    public class JobCardDependency
    {
        public int Id { get; set; }

        // Dependent Job Card (the one that's waiting)
        public int DependentJobCardId { get; set; }
        public string? DependentJobCardNo { get; set; }

        // Prerequisite Job Card (the one that must complete first)
        public int PrerequisiteJobCardId { get; set; }
        public string? PrerequisiteJobCardNo { get; set; }

        // Dependency Type
        public string DependencyType { get; set; } = "Sequential";

        // Status
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // Lag Time (optional delay after prerequisite completes)
        public int? LagTimeMinutes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
