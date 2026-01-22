using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating an existing BOM
    /// </summary>
    public class UpdateBOMRequest
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50)]
        public string? BOMNo { get; set; }

        [StringLength(20)]
        public string? RevisionNumber { get; set; }

        public DateTime? RevisionDate { get; set; }

        [StringLength(50)]
        public string? BOMType { get; set; }

        public decimal? BaseQuantity { get; set; }

        [StringLength(10)]
        public string? BaseUOM { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
