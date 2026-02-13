using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MachineModelResponse
    {
        public int Id { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; } // Number of products using this model
    }
}
