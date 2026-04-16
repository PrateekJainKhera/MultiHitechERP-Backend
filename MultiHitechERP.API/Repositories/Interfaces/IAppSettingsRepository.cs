namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IAppSettingsRepository
    {
        Task<string?> GetValueAsync(string key);
        Task SetValueAsync(string key, string value, string? updatedBy = null);
        Task<IEnumerable<Models.AppSetting>> GetAllAsync();
    }
}
