using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a new BOM
    /// </summary>
    public class CreateBOMRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [StringLength(50)]
        public string? BOMNo { get; set; }

        [StringLength(20)]
        public string? RevisionNumber { get; set; }

        [StringLength(50)]
        public string? BOMType { get; set; } = "Manufacturing";

        public decimal? BaseQuantity { get; set; } = 1;

        [StringLength(10)]
        public string? BaseUOM { get; set; } = "PCS";

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }
}
