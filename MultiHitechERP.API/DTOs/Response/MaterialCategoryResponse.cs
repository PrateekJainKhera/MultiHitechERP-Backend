namespace MultiHitechERP.API.DTOs.Response
{
    public class MaterialCategoryResponse
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DefaultUOM { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
