namespace MultiHitechERP.API.Models.Masters
{
    public class MaterialCategory
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DefaultUOM { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty; // 'raw_material' or 'component'
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
