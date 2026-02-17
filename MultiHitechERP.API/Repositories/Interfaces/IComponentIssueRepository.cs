using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IComponentIssueRepository
    {
        Task<int> CreateAsync(ComponentIssue issue);
        Task<IEnumerable<ComponentIssue>> GetAllAsync();
        Task<IEnumerable<ComponentIssue>> GetByComponentIdAsync(int componentId);
        Task<IEnumerable<ComponentWithStockResponse>> GetComponentsWithStockAsync();
    }
}
