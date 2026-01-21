using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Material Issue operations (physical issuance tracking)
    /// </summary>
    public interface IMaterialIssueRepository
    {
        // Basic CRUD Operations
        Task<MaterialIssue?> GetByIdAsync(Guid id);
        Task<MaterialIssue?> GetByIssueNoAsync(string issueNo);
        Task<IEnumerable<MaterialIssue>> GetAllAsync();
        Task<IEnumerable<MaterialIssue>> GetByRequisitionIdAsync(Guid requisitionId);
        Task<IEnumerable<MaterialIssue>> GetByJobCardNoAsync(string jobCardNo);
        Task<IEnumerable<MaterialIssue>> GetByOrderNoAsync(string orderNo);

        // Create, Update, Delete
        Task<Guid> InsertAsync(MaterialIssue issue);
        Task<bool> UpdateAsync(MaterialIssue issue);
        Task<bool> DeleteAsync(Guid id);

        // Queries
        Task<IEnumerable<MaterialIssue>> GetByStatusAsync(string status);
        Task<IEnumerable<MaterialIssue>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MaterialIssue>> GetByIssuedByAsync(string issuedById);
        Task<IEnumerable<MaterialIssue>> GetByReceivedByAsync(string receivedById);

        // Statistics
        Task<decimal> GetTotalIssuedQuantityAsync(Guid requisitionId);
    }
}
