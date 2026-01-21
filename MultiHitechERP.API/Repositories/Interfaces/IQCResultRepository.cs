using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Quality;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Quality Control Results
    /// Manages QC inspection records, defects, and approval workflow
    /// </summary>
    public interface IQCResultRepository
    {
        // Basic CRUD Operations
        Task<QCResult?> GetByIdAsync(Guid id);
        Task<IEnumerable<QCResult>> GetAllAsync();
        Task<Guid> InsertAsync(QCResult qcResult);
        Task<bool> UpdateAsync(QCResult qcResult);
        Task<bool> DeleteAsync(Guid id);

        // QC-specific Queries
        Task<IEnumerable<QCResult>> GetByJobCardIdAsync(Guid jobCardId);
        Task<IEnumerable<QCResult>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<QCResult>> GetByInspectionTypeAsync(string inspectionType);
        Task<IEnumerable<QCResult>> GetByQCStatusAsync(string qcStatus);
        Task<IEnumerable<QCResult>> GetByInspectorAsync(string inspectorName);
        Task<IEnumerable<QCResult>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Defect Tracking
        Task<IEnumerable<QCResult>> GetDefectiveResultsAsync();
        Task<IEnumerable<QCResult>> GetByDefectCategoryAsync(string category);
        Task<IEnumerable<QCResult>> GetReworkRequiredAsync();

        // Status & Approval Operations
        Task<bool> UpdateQCStatusAsync(Guid id, string qcStatus);
        Task<bool> ApproveQCResultAsync(Guid id, string approvedBy, DateTime approvedAt);
        Task<IEnumerable<QCResult>> GetPendingQCAsync();
        Task<IEnumerable<QCResult>> GetFailedQCAsync();
        Task<IEnumerable<QCResult>> GetApprovedResultsAsync();

        // Statistics
        Task<int> GetTotalInspectedQuantityForJobCardAsync(Guid jobCardId);
        Task<int> GetTotalPassedQuantityForJobCardAsync(Guid jobCardId);
        Task<int> GetTotalRejectedQuantityForJobCardAsync(Guid jobCardId);
        Task<decimal> GetPassRateForJobCardAsync(Guid jobCardId);
        Task<decimal> GetOverallPassRateAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Latest Result
        Task<QCResult?> GetLatestResultForJobCardAsync(Guid jobCardId);
    }
}
