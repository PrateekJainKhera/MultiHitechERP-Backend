using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a machine model master record (e.g., "Roller Mill 500", "Flexo Press 8-Color")
    /// Products are associated with machine models
    /// </summary>
    public class MachineModel
    {
        public int Id { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
