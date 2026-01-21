using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Quality;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Quality Control business logic
    /// Manages QC inspections, defect tracking, and approval workflow
    /// </summary>
    public interface IQualityService
    {
        // Basic CRUD Operations
        Task<ApiResponse<QCResult>> GetByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<QCResult>>> GetAllAsync();
        Task<ApiResponse<Guid>> CreateQCResultAsync(QCResult qcResult);
        Task<ApiResponse<bool>> UpdateQCResultAsync(QCResult qcResult);
        Task<ApiResponse<bool>> DeleteQCResultAsync(Guid id);

        // QC-specific Queries
        Task<ApiResponse<IEnumerable<QCResult>>> GetByJobCardIdAsync(Guid jobCardId);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByOrderIdAsync(Guid orderId);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByInspectionTypeAsync(string inspectionType);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByInspectorAsync(string inspectorName);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Defect Tracking
        Task<ApiResponse<IEnumerable<QCResult>>> GetDefectiveResultsAsync();
        Task<ApiResponse<IEnumerable<QCResult>>> GetByDefectCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<QCResult>>> GetReworkRequiredAsync();

        // QC Operations
        Task<ApiResponse<Guid>> RecordInspectionAsync(Guid jobCardId, int quantityInspected, int quantityPassed, int quantityRejected, int? quantityRework, string inspectedBy, string inspectionType, string? defectDescription = null, string? defectCategory = null);
        Task<ApiResponse<bool>> UpdateQCStatusAsync(Guid id, string status);
        Task<ApiResponse<bool>> ApproveQCResultAsync(Guid id, string approvedBy);
        Task<ApiResponse<bool>> RejectQCResultAsync(Guid id, string rejectionReason);

        // Approval Workflow
        Task<ApiResponse<IEnumerable<QCResult>>> GetPendingApprovalsAsync();
        Task<ApiResponse<IEnumerable<QCResult>>> GetApprovedResultsAsync();
        Task<ApiResponse<IEnumerable<QCResult>>> GetFailedQCAsync();

        // Statistics
        Task<ApiResponse<int>> GetTotalInspectedQuantityForJobCardAsync(Guid jobCardId);
        Task<ApiResponse<int>> GetTotalPassedQuantityForJobCardAsync(Guid jobCardId);
        Task<ApiResponse<int>> GetTotalRejectedQuantityForJobCardAsync(Guid jobCardId);
        Task<ApiResponse<decimal>> GetPassRateForJobCardAsync(Guid jobCardId);
        Task<ApiResponse<decimal>> GetOverallPassRateAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Latest Result
        Task<ApiResponse<QCResult>> GetLatestResultForJobCardAsync(Guid jobCardId);
    }
}
