using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Quality
{
    /// <summary>
    /// Quality Control API endpoints
    /// Handles QC inspections, defect tracking, and approval workflow
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QualityController : ControllerBase
    {
        private readonly IQualityService _service;

        public QualityController(IQualityService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all QC results
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC result by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<QCResultResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get QC results by job card ID
        /// </summary>
        [HttpGet("by-job-card/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByJobCardId(Guid jobCardId)
        {
            var response = await _service.GetByJobCardIdAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC results by order ID
        /// </summary>
        [HttpGet("by-order/{orderId:guid}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByOrderId(Guid orderId)
        {
            var response = await _service.GetByOrderIdAsync(orderId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC results by inspection type
        /// </summary>
        [HttpGet("by-inspection-type/{inspectionType}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByInspectionType(string inspectionType)
        {
            var response = await _service.GetByInspectionTypeAsync(inspectionType);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC results by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC results by inspector
        /// </summary>
        [HttpGet("by-inspector/{inspectorName}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByInspector(string inspectorName)
        {
            var response = await _service.GetByInspectorAsync(inspectorName);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC results by date range
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var response = await _service.GetByDateRangeAsync(startDate, endDate);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get all defective results (with rejections or rework)
        /// </summary>
        [HttpGet("defective")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetDefectiveResults()
        {
            var response = await _service.GetDefectiveResultsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get QC results by defect category
        /// </summary>
        [HttpGet("by-defect-category/{category}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetByDefectCategory(string category)
        {
            var response = await _service.GetByDefectCategoryAsync(category);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get all results requiring rework
        /// </summary>
        [HttpGet("rework-required")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetReworkRequired()
        {
            var response = await _service.GetReworkRequiredAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get pending QC approvals
        /// </summary>
        [HttpGet("pending-approvals")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetPendingApprovals()
        {
            var response = await _service.GetPendingApprovalsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get approved QC results
        /// </summary>
        [HttpGet("approved")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetApprovedResults()
        {
            var response = await _service.GetApprovedResultsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get failed QC results
        /// </summary>
        [HttpGet("failed")]
        public async Task<ActionResult<ApiResponse<QCResultResponse[]>>> GetFailedResults()
        {
            var response = await _service.GetFailedQCAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<QCResultResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get latest QC result for a job card
        /// </summary>
        [HttpGet("latest/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<QCResultResponse>>> GetLatestForJobCard(Guid jobCardId)
        {
            var response = await _service.GetLatestResultForJobCardAsync(jobCardId);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<QCResultResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Record a quality inspection
        /// </summary>
        [HttpPost("record-inspection")]
        public async Task<ActionResult<ApiResponse<Guid>>> RecordInspection([FromBody] RecordInspectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.RecordInspectionAsync(
                request.JobCardId,
                request.QuantityInspected,
                request.QuantityPassed,
                request.QuantityRejected,
                request.QuantityRework,
                request.InspectedBy,
                request.InspectionType,
                request.DefectDescription,
                request.DefectCategory);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update QC status
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateQCStatus(Guid id, [FromBody] UpdateQCStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateQCStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Approve a QC result
        /// </summary>
        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveQCResult(Guid id, [FromBody] ApproveQCRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.ApproveQCResultAsync(id, request.ApprovedBy);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Reject a QC result
        /// </summary>
        [HttpPost("{id:guid}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> RejectQCResult(Guid id, [FromBody] RejectQCRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.RejectQCResultAsync(id, request.RejectionReason);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get total inspected quantity for a job card
        /// </summary>
        [HttpGet("statistics/total-inspected/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalInspectedQuantity(Guid jobCardId)
        {
            var response = await _service.GetTotalInspectedQuantityForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get total passed quantity for a job card
        /// </summary>
        [HttpGet("statistics/total-passed/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalPassedQuantity(Guid jobCardId)
        {
            var response = await _service.GetTotalPassedQuantityForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get total rejected quantity for a job card
        /// </summary>
        [HttpGet("statistics/total-rejected/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalRejectedQuantity(Guid jobCardId)
        {
            var response = await _service.GetTotalRejectedQuantityForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get pass rate for a job card (percentage)
        /// </summary>
        [HttpGet("statistics/pass-rate/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetPassRate(Guid jobCardId)
        {
            var response = await _service.GetPassRateForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get overall pass rate (optionally filtered by date range)
        /// </summary>
        [HttpGet("statistics/overall-pass-rate")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetOverallPassRate(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var response = await _service.GetOverallPassRateAsync(startDate, endDate);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a QC result record
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteQCResultAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Helper Methods
        private static QCResultResponse MapToResponse(Models.Quality.QCResult qcResult)
        {
            return new QCResultResponse
            {
                Id = qcResult.Id,
                JobCardId = qcResult.JobCardId,
                JobCardNo = qcResult.JobCardNo,
                OrderId = qcResult.OrderId,
                OrderNo = qcResult.OrderNo,
                InspectionType = qcResult.InspectionType,
                InspectionDate = qcResult.InspectionDate,
                InspectedBy = qcResult.InspectedBy,
                QuantityInspected = qcResult.QuantityInspected,
                QuantityPassed = qcResult.QuantityPassed,
                QuantityRejected = qcResult.QuantityRejected,
                QuantityRework = qcResult.QuantityRework,
                QCStatus = qcResult.QCStatus,
                DefectDescription = qcResult.DefectDescription,
                DefectCategory = qcResult.DefectCategory,
                RejectionReason = qcResult.RejectionReason,
                MeasurementData = qcResult.MeasurementData,
                CorrectiveAction = qcResult.CorrectiveAction,
                RequiresRework = qcResult.RequiresRework,
                ApprovedBy = qcResult.ApprovedBy,
                ApprovedAt = qcResult.ApprovedAt,
                Remarks = qcResult.Remarks,
                CreatedAt = qcResult.CreatedAt,
                CreatedBy = qcResult.CreatedBy
            };
        }
    }
}
