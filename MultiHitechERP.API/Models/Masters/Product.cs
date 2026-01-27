using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a product master record (rollers for flexo printing)
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string RollerType { get; set; } = string.Empty;

        // Dimensions
        public decimal Diameter { get; set; }
        public decimal Length { get; set; }

        // Material & Finish
        public string? MaterialGrade { get; set; }
        public string? SurfaceFinish { get; set; }
        public string? Hardness { get; set; }

        // Drawing Reference
        public string? DrawingNo { get; set; }
        public string? RevisionNo { get; set; }
        public string? RevisionDate { get; set; }

        // Additional Properties
        public int? NumberOfTeeth { get; set; }

        // Process Reference
        public int ProcessTemplateId { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
