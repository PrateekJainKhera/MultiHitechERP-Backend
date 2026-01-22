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
        Task<ApiResponse<QCResult>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<QCResult>>> GetAllAsync();
        Task<ApiResponse<int>> CreateQCResultAsync(QCResult qcResult);
        Task<ApiResponse<bool>> UpdateQCResultAsync(QCResult qcResult);
        Task<ApiResponse<bool>> DeleteQCResultAsync(int id);

        // QC-specific Queries
        Task<ApiResponse<IEnumerable<QCResult>>> GetByJobCardIdAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByOrderIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByInspectionTypeAsync(string inspectionType);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByInspectorAsync(string inspectorName);
        Task<ApiResponse<IEnumerable<QCResult>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Defect Tracking
        Task<ApiResponse<IEnumerable<QCResult>>> GetDefectiveResultsAsync();
        Task<ApiResponse<IEnumerable<QCResult>>> GetByDefectCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<QCResult>>> GetReworkRequiredAsync();

        // QC Operations
        Task<ApiResponse<int>> RecordInspectionAsync(int jobCardId, int quantityInspected, int quantityPassed, int quantityRejected, int? quantityRework, string inspectedBy, string inspectionType, string? defectDescription = null, string? defectCategory = null);
        Task<ApiResponse<bool>> UpdateQCStatusAsync(int id, string status);
        Task<ApiResponse<bool>> ApproveQCResultAsync(int id, string approvedBy);
        Task<ApiResponse<bool>> RejectQCResultAsync(int id, string rejectionReason);

        // Approval Workflow
        Task<ApiResponse<IEnumerable<QCResult>>> GetPendingApprovalsAsync();
        Task<ApiResponse<IEnumerable<QCResult>>> GetApprovedResultsAsync();
        Task<ApiResponse<IEnumerable<QCResult>>> GetFailedQCAsync();

        // Statistics
        Task<ApiResponse<int>> GetTotalInspectedQuantityForJobCardAsync(int jobCardId);
        Task<ApiResponse<int>> GetTotalPassedQuantityForJobCardAsync(int jobCardId);
        Task<ApiResponse<int>> GetTotalRejectedQuantityForJobCardAsync(int jobCardId);
        Task<ApiResponse<decimal>> GetPassRateForJobCardAsync(int jobCardId);
        Task<ApiResponse<decimal>> GetOverallPassRateAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Latest Result
        Task<ApiResponse<QCResult>> GetLatestResultForJobCardAsync(int jobCardId);
    }
}
