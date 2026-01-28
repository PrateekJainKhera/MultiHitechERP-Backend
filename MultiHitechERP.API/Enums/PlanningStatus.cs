namespace MultiHitechERP.API.Enums
{
    /// <summary>
    /// Represents the planning status of an order (matches frontend PlanningStatus enum)
    /// </summary>
    public enum PlanningStatus
    {
        NotPlanned,    // "Not Planned" - default, no job cards generated yet
        Planned,       // "Planned" - job cards created and assigned
        Released       // "Released" - approved for production execution
    }
}
