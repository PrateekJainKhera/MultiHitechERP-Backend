using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating stock level parameters
    /// </summary>
    public class UpdateStockLevelsRequest
    {
        [Required]
        public int MaterialId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinimumStock { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaximumStock { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ReorderLevel { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ReorderQuantity { get; set; }
    }
}
