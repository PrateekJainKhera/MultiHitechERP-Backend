using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Quality;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// QualityService implementation with comprehensive QC logic
    /// Handles inspection recording, defect tracking, and approval workflow
    /// </summary>
    public class QualityService : IQualityService
    {
        private readonly IQCResultRepository _qcResultRepository;
        private readonly IJobCardRepository _jobCardRepository;

        public QualityService(
            IQCResultRepository qcResultRepository,
            IJobCardRepository jobCardRepository)
        {
            _qcResultRepository = qcResultRepository;
            _jobCardRepository = jobCardRepository;
        }

        public async Task<ApiResponse<QCResult>> GetByIdAsync(Guid id)
        {
            var result = await _qcResultRepository.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<QCResult>.ErrorResponse("QC Result not found");

            return ApiResponse<QCResult>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetAllAsync()
        {
            var results = await _qcResultRepository.GetAllAsync();
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<Guid>> CreateQCResultAsync(QCResult qcResult)
        {
            var id = await _qcResultRepository.InsertAsync(qcResult);
            return ApiResponse<Guid>.SuccessResponse(id, "QC Result created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateQCResultAsync(QCResult qcResult)
        {
            var existing = await _qcResultRepository.GetByIdAsync(qcResult.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("QC Result not found");

            var success = await _qcResultRepository.UpdateAsync(qcResult);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update QC Result");

            return ApiResponse<bool>.SuccessResponse(true, "QC Result updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteQCResultAsync(Guid id)
        {
            var existing = await _qcResultRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("QC Result not found");

            var success = await _qcResultRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete QC Result");

            return ApiResponse<bool>.SuccessResponse(true, "QC Result deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByJobCardIdAsync(Guid jobCardId)
        {
            var results = await _qcResultRepository.GetByJobCardIdAsync(jobCardId);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByOrderIdAsync(Guid orderId)
        {
            var results = await _qcResultRepository.GetByOrderIdAsync(orderId);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByInspectionTypeAsync(string inspectionType)
        {
            if (string.IsNullOrWhiteSpace(inspectionType))
                return ApiResponse<IEnumerable<QCResult>>.ErrorResponse("Inspection type is required");

            var results = await _qcResultRepository.GetByInspectionTypeAsync(inspectionType);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<QCResult>>.ErrorResponse("Status is required");

            var results = await _qcResultRepository.GetByQCStatusAsync(status);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByInspectorAsync(string inspectorName)
        {
            if (string.IsNullOrWhiteSpace(inspectorName))
                return ApiResponse<IEnumerable<QCResult>>.ErrorResponse("Inspector name is required");

            var results = await _qcResultRepository.GetByInspectorAsync(inspectorName);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return ApiResponse<IEnumerable<QCResult>>.ErrorResponse("Start date must be before end date");

            var results = await _qcResultRepository.GetByDateRangeAsync(startDate, endDate);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetDefectiveResultsAsync()
        {
            var results = await _qcResultRepository.GetDefectiveResultsAsync();
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetByDefectCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return ApiResponse<IEnumerable<QCResult>>.ErrorResponse("Defect category is required");

            var results = await _qcResultRepository.GetByDefectCategoryAsync(category);
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetReworkRequiredAsync()
        {
            var results = await _qcResultRepository.GetReworkRequiredAsync();
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<Guid>> RecordInspectionAsync(
            Guid jobCardId,
            int quantityInspected,
            int quantityPassed,
            int quantityRejected,
            int? quantityRework,
            string inspectedBy,
            string inspectionType,
            string? defectDescription = null,
            string? defectCategory = null)
        {
            // Validate job card exists
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return ApiResponse<Guid>.ErrorResponse("Job card not found");

            // Validate quantities
            if (quantityInspected <= 0)
                return ApiResponse<Guid>.ErrorResponse("Quantity inspected must be greater than zero");

            if (quantityPassed + quantityRejected + (quantityRework ?? 0) != quantityInspected)
                return ApiResponse<Guid>.ErrorResponse("Sum of passed, rejected, and rework quantities must equal total inspected");

            // Determine QC status based on results
            string qcStatus;
            if (quantityRejected > 0)
                qcStatus = "Failed";
            else if (quantityRework > 0)
                qcStatus = "Rework Required";
            else if (quantityPassed == quantityInspected)
                qcStatus = "Passed";
            else
                qcStatus = "Pending";

            // Create QC result record
            var qcResult = new QCResult
            {
                JobCardId = jobCardId,
                JobCardNo = jobCard.JobCardNo,
                OrderId = jobCard.OrderId,
                OrderNo = jobCard.OrderNo,
                InspectionType = inspectionType,
                InspectionDate = DateTime.UtcNow,
                InspectedBy = inspectedBy,
                QuantityInspected = quantityInspected,
                QuantityPassed = quantityPassed,
                QuantityRejected = quantityRejected,
                QuantityRework = quantityRework,
                QCStatus = qcStatus,
                DefectDescription = defectDescription,
                DefectCategory = defectCategory,
                RequiresRework = quantityRework > 0
            };

            var id = await _qcResultRepository.InsertAsync(qcResult);

            // Update job card QC status
            if (qcStatus == "Passed")
            {
                // await _jobCardRepository.UpdateQCStatusAsync(jobCardId, "QC Approved");
            }
            else if (qcStatus == "Failed")
            {
                // await _jobCardRepository.UpdateQCStatusAsync(jobCardId, "QC Rejected");
            }
            else if (qcStatus == "Rework Required")
            {
                // await _jobCardRepository.UpdateQCStatusAsync(jobCardId, "Rework");
            }

            return ApiResponse<Guid>.SuccessResponse(id, $"Inspection recorded successfully - Status: {qcStatus}");
        }

        public async Task<ApiResponse<bool>> UpdateQCStatusAsync(Guid id, string status)
        {
            var existing = await _qcResultRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("QC Result not found");

            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<bool>.ErrorResponse("Status is required");

            var success = await _qcResultRepository.UpdateQCStatusAsync(id, status);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update QC status");

            // Update associated job card status
            if (status == "Passed" || status == "Approved")
            {
                // await _jobCardRepository.UpdateQCStatusAsync(existing.JobCardId, "QC Approved");
            }
            else if (status == "Failed")
            {
                // await _jobCardRepository.UpdateQCStatusAsync(existing.JobCardId, "QC Rejected");
            }

            return ApiResponse<bool>.SuccessResponse(true, "QC status updated successfully");
        }

        public async Task<ApiResponse<bool>> ApproveQCResultAsync(Guid id, string approvedBy)
        {
            var existing = await _qcResultRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("QC Result not found");

            if (existing.QCStatus == "Failed")
                return ApiResponse<bool>.ErrorResponse("Cannot approve a failed QC result");

            if (string.IsNullOrWhiteSpace(approvedBy))
                return ApiResponse<bool>.ErrorResponse("Approver name is required");

            var success = await _qcResultRepository.ApproveQCResultAsync(id, approvedBy, DateTime.UtcNow);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to approve QC result");

            // Update job card status to approved
            // await _jobCardRepository.UpdateQCStatusAsync(existing.JobCardId, "QC Approved");

            return ApiResponse<bool>.SuccessResponse(true, "QC result approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectQCResultAsync(Guid id, string rejectionReason)
        {
            var existing = await _qcResultRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("QC Result not found");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                return ApiResponse<bool>.ErrorResponse("Rejection reason is required");

            // Update the QC result with rejection details
            existing.QCStatus = "Failed";
            existing.RejectionReason = rejectionReason;
            existing.RequiresRework = true;

            var success = await _qcResultRepository.UpdateAsync(existing);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to reject QC result");

            // Update job card status to rejected
            // await _jobCardRepository.UpdateQCStatusAsync(existing.JobCardId, "QC Rejected");

            return ApiResponse<bool>.SuccessResponse(true, "QC result rejected successfully");
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetPendingApprovalsAsync()
        {
            var results = await _qcResultRepository.GetPendingQCAsync();
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetApprovedResultsAsync()
        {
            var results = await _qcResultRepository.GetApprovedResultsAsync();
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<IEnumerable<QCResult>>> GetFailedQCAsync()
        {
            var results = await _qcResultRepository.GetFailedQCAsync();
            return ApiResponse<IEnumerable<QCResult>>.SuccessResponse(results);
        }

        public async Task<ApiResponse<int>> GetTotalInspectedQuantityForJobCardAsync(Guid jobCardId)
        {
            var total = await _qcResultRepository.GetTotalInspectedQuantityForJobCardAsync(jobCardId);
            return ApiResponse<int>.SuccessResponse(total);
        }

        public async Task<ApiResponse<int>> GetTotalPassedQuantityForJobCardAsync(Guid jobCardId)
        {
            var total = await _qcResultRepository.GetTotalPassedQuantityForJobCardAsync(jobCardId);
            return ApiResponse<int>.SuccessResponse(total);
        }

        public async Task<ApiResponse<int>> GetTotalRejectedQuantityForJobCardAsync(Guid jobCardId)
        {
            var total = await _qcResultRepository.GetTotalRejectedQuantityForJobCardAsync(jobCardId);
            return ApiResponse<int>.SuccessResponse(total);
        }

        public async Task<ApiResponse<decimal>> GetPassRateForJobCardAsync(Guid jobCardId)
        {
            var passRate = await _qcResultRepository.GetPassRateForJobCardAsync(jobCardId);
            return ApiResponse<decimal>.SuccessResponse(passRate);
        }

        public async Task<ApiResponse<decimal>> GetOverallPassRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var passRate = await _qcResultRepository.GetOverallPassRateAsync(startDate, endDate);
            return ApiResponse<decimal>.SuccessResponse(passRate);
        }

        public async Task<ApiResponse<QCResult>> GetLatestResultForJobCardAsync(Guid jobCardId)
        {
            var result = await _qcResultRepository.GetLatestResultForJobCardAsync(jobCardId);
            if (result == null)
                return ApiResponse<QCResult>.ErrorResponse("No QC result found for this job card");

            return ApiResponse<QCResult>.SuccessResponse(result);
        }
    }
}
