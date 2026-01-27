using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateMaterialCategoryRequest
    {
        [Required(ErrorMessage = "Category code is required")]
        [StringLength(50, ErrorMessage = "Category code cannot exceed 50 characters")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(200, ErrorMessage = "Category name cannot exceed 200 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quality is required")]
        [StringLength(100, ErrorMessage = "Quality cannot exceed 100 characters")]
        public string Quality { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Default UOM is required")]
        [StringLength(20, ErrorMessage = "Default UOM cannot exceed 20 characters")]
        public string DefaultUOM { get; set; } = string.Empty;

        [Required(ErrorMessage = "Material type is required")]
        public string MaterialType { get; set; } = string.Empty; // 'raw_material' or 'component'

        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
    }
}
