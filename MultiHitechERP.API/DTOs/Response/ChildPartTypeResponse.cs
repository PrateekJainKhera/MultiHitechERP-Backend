using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ChildPartTypeResponse
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
