using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    // One product spec combo (Model · Roller · Teeth) with its order stats.
    public class MachineModelRow
    {
        public string ModelName { get; set; } = string.Empty;
        public string? RollerType { get; set; }
        public int? NumberOfTeeth { get; set; }
        public int Orders { get; set; }
        public int TotalQty { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    // Distinct model name for the search list.
    public class MachineModelName
    {
        public string ModelName { get; set; } = string.Empty;
        public int Orders { get; set; }
        public int TotalQty { get; set; }
    }

    public class MachineModelsResponse
    {
        public List<MachineModelRow> Top10 { get; set; } = new();
        public List<MachineModelName> Models { get; set; } = new();
    }

    public class ModelPeriodCount
    {
        public string Period { get; set; } = string.Empty; // yyyy-MM or yyyy
        public int Orders { get; set; }
        public int Qty { get; set; }
    }

    public class MachineModelDetailResponse
    {
        public string ModelName { get; set; } = string.Empty;
        public List<MachineModelRow> Variants { get; set; } = new(); // roller/teeth combos of this model
        public List<ModelPeriodCount> Monthly { get; set; } = new();  // last 24 months
        public List<ModelPeriodCount> Yearly { get; set; } = new();
        public int TotalOrders { get; set; }
        public int TotalQty { get; set; }
    }

    public class MachineModelCustomerRow
    {
        public string CustomerName { get; set; } = string.Empty;
        public int Orders { get; set; }
        public int TotalQty { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}
