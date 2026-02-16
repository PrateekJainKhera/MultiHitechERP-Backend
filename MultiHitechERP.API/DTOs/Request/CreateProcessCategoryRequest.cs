using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a new process category
    /// CategoryCode is auto-generated from CategoryName
    /// </summary>
    public class CreateProcessCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an existing process category
    /// CategoryCode can be edited during update
    /// </summary>
    public class UpdateProcessCategoryRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category code is required")]
        [StringLength(50, ErrorMessage = "Category code cannot exceed 50 characters")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public string? UpdatedBy { get; set; }
    }
}
