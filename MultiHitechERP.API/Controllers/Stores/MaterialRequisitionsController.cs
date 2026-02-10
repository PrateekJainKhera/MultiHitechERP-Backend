using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    /// <summary>
    /// Material Requisition management API endpoints
    /// Handles requisition workflow and material allocation
    /// </summary>
    [ApiController]
    [Route("api/stores/[controller]")]
    public class MaterialRequisitionsController : ControllerBase
    {
        private readonly IMaterialRequisitionService _service;

        public MaterialRequisitionsController(IMaterialRequisitionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all material requisitions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get material requisition by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse>>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<MaterialRequisitionResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get material requisition items by requisition ID
        /// </summary>
        [HttpGet("{id:int}/items")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionItemResponse[]>>> GetItems(int id)
        {
            var response = await _service.GetRequisitionItemsAsync(id);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToItemResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionItemResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get material requisition by requisition number
        /// </summary>
        [HttpGet("by-requisition-no/{requisitionNo}")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse>>> GetByRequisitionNo(string requisitionNo)
        {
            var response = await _service.GetByRequisitionNoAsync(requisitionNo);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<MaterialRequisitionResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get material requisitions by job card ID
        /// </summary>
        [HttpGet("by-job-card/{jobCardId:int}")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetByJobCardId(int jobCardId)
        {
            var response = await _service.GetByJobCardIdAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get material requisitions by order ID
        /// </summary>
        [HttpGet("by-order/{orderId:int}")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetByOrderId(int orderId)
        {
            var response = await _service.GetByOrderIdAsync(orderId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get material requisitions by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get pending material requisitions
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetPendingRequisitions()
        {
            var response = await _service.GetPendingRequisitionsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get approved material requisitions
        /// </summary>
        [HttpGet("approved")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetApprovedRequisitions()
        {
            var response = await _service.GetApprovedRequisitionsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get material requisitions by priority
        /// </summary>
        [HttpGet("by-priority/{priority}")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetByPriority(string priority)
        {
            var response = await _service.GetByPriorityAsync(priority);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get overdue material requisitions
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<ApiResponse<MaterialRequisitionResponse[]>>> GetOverdueRequisitions()
        {
            var response = await _service.GetOverdueRequisitionsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<MaterialRequisitionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Create a new material requisition
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateMaterialRequisitionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<int>.ErrorResponse("Invalid request data"));

            var requisition = new MaterialRequisition
            {
                RequisitionNo = request.RequisitionNo,
                RequisitionDate = request.RequisitionDate,
                JobCardId = request.JobCardId,
                JobCardNo = request.JobCardNo,
                OrderId = request.OrderId,
                OrderNo = request.OrderNo,
                CustomerName = request.CustomerName,
                Priority = request.Priority,
                DueDate = request.DueDate,
                RequestedBy = request.RequestedBy,
                Remarks = request.Remarks,
                CreatedBy = request.CreatedBy
            };

            // Check if items are provided
            if (request.Items != null && request.Items.Count > 0)
            {
                // Map request items to model items
                var items = request.Items.Select(item => new MaterialRequisitionItem
                {
                    MaterialId = item.MaterialId,
                    MaterialCode = item.MaterialCode,
                    MaterialName = item.MaterialName,
                    MaterialGrade = item.MaterialGrade,
                    QuantityRequired = item.QuantityRequired,
                    UOM = item.UOM,
                    LengthRequiredMM = item.LengthRequiredMM,
                    DiameterMM = item.DiameterMM,
                    NumberOfPieces = item.NumberOfPieces,
                    JobCardId = item.JobCardId,
                    JobCardNo = item.JobCardNo,
                    ProcessId = item.ProcessId,
                    ProcessName = item.ProcessName,
                    SelectedPieceIds = item.SelectedPieceIds != null && item.SelectedPieceIds.Count > 0
                        ? string.Join(",", item.SelectedPieceIds)
                        : null,
                    Remarks = item.Remarks
                }).ToList();

                var response = await _service.CreateRequisitionWithItemsAsync(requisition, items);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            else
            {
                var response = await _service.CreateRequisitionAsync(requisition);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
        }

        /// <summary>
        /// Update an existing material requisition
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(int id, [FromBody] UpdateMaterialRequisitionRequest request)
        {
            if (id != request.Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var requisition = new MaterialRequisition
            {
                Id = request.Id,
                RequisitionNo = request.RequisitionNo,
                RequisitionDate = request.RequisitionDate,
                JobCardId = request.JobCardId,
                JobCardNo = request.JobCardNo,
                OrderId = request.OrderId,
                OrderNo = request.OrderNo,
                CustomerName = request.CustomerName,
                Status = request.Status,
                Priority = request.Priority,
                DueDate = request.DueDate,
                RequestedBy = request.RequestedBy,
                ApprovedBy = request.ApprovedBy,
                ApprovalDate = request.ApprovalDate,
                Remarks = request.Remarks
            };

            var response = await _service.UpdateRequisitionAsync(requisition);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a material requisition
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _service.DeleteRequisitionAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Approve a material requisition
        /// </summary>
        [HttpPost("{id:int}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> Approve(int id, [FromBody] ApproveRequisitionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.ApproveRequisitionAsync(id, request.ApprovedBy);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Reject a material requisition
        /// </summary>
        [HttpPost("{id:int}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> Reject(int id, [FromBody] RejectRequisitionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.RejectRequisitionAsync(id, request.RejectedBy, request.Reason);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update selected piece IDs for a requisition item (only if not set during planning)
        /// </summary>
        [HttpPatch("{id:int}/items/{itemId:int}/selected-pieces")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateItemSelectedPieces(int id, int itemId, [FromBody] UpdateItemSelectedPiecesRequest request)
        {
            var response = await _service.UpdateItemSelectedPiecesAsync(id, itemId, request.PieceIds);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update requisition status
        /// </summary>
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Allocate materials to requisition using FIFO
        /// </summary>
        [HttpPost("{id:int}/allocate")]
        public async Task<ActionResult<ApiResponse<bool>>> AllocateMaterials(int id, [FromBody] AllocateMaterialRequest request)
        {
            if (id != request.RequisitionId)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.AllocateMaterialsAsync(
                request.RequisitionId,
                request.MaterialId,
                request.RequiredQuantityMM);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Deallocate materials from requisition
        /// </summary>
        [HttpPost("{id:int}/deallocate")]
        public async Task<ActionResult<ApiResponse<bool>>> DeallocateMaterials(int id)
        {
            var response = await _service.DeallocateMaterialsAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Issue allocated materials to production
        /// </summary>
        [HttpPost("{id:int}/issue")]
        public async Task<ActionResult<ApiResponse<Guid>>> IssueMaterials(int id, [FromBody] IssueMaterialRequest request)
        {
            if (id != request.RequisitionId)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("ID mismatch"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.IssueMaterialsAsync(
                request.RequisitionId,
                request.JobCardId,
                request.IssuedBy,
                request.ReceivedBy);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get allocated material pieces for requisition
        /// </summary>
        [HttpGet("{id:int}/allocated-pieces")]
        public async Task<ActionResult<ApiResponse<MaterialPieceResponse[]>>> GetAllocatedPieces(int id)
        {
            var response = await _service.GetAllocatedPiecesAsync(id);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToPieceResponse).ToArray();
            return Ok(ApiResponse<MaterialPieceResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get issuance history for requisition
        /// </summary>
        [HttpGet("{id:int}/issuance-history")]
        public async Task<ActionResult<ApiResponse<MaterialIssueResponse[]>>> GetIssuanceHistory(int id)
        {
            var response = await _service.GetIssuanceHistoryAsync(id);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToIssueResponse).ToArray();
            return Ok(ApiResponse<MaterialIssueResponse[]>.SuccessResponse(dtos));
        }

        // Helper Methods
        private static MaterialRequisitionResponse MapToResponse(MaterialRequisition requisition)
        {
            return new MaterialRequisitionResponse
            {
                Id = requisition.Id,
                RequisitionNo = requisition.RequisitionNo,
                RequisitionDate = requisition.RequisitionDate,
                JobCardId = requisition.JobCardId,
                JobCardNo = requisition.JobCardNo,
                OrderId = requisition.OrderId,
                OrderNo = requisition.OrderNo,
                CustomerName = requisition.CustomerName,
                Status = requisition.Status,
                Priority = requisition.Priority,
                DueDate = requisition.DueDate,
                RequestedBy = requisition.RequestedBy,
                ApprovedBy = requisition.ApprovedBy,
                ApprovalDate = requisition.ApprovalDate,
                Remarks = requisition.Remarks,
                CreatedAt = requisition.CreatedAt,
                CreatedBy = requisition.CreatedBy
            };
        }

        private static MaterialRequisitionItemResponse MapToItemResponse(MaterialRequisitionItem item)
        {
            // Parse comma-separated SelectedPieceIds back to List<int>
            List<int>? selectedPieceIds = null;
            if (!string.IsNullOrWhiteSpace(item.SelectedPieceIds))
            {
                try
                {
                    selectedPieceIds = item.SelectedPieceIds
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList();
                }
                catch
                {
                    // If parsing fails, leave as null
                    selectedPieceIds = null;
                }
            }

            return new MaterialRequisitionItemResponse
            {
                Id = item.Id,
                RequisitionId = item.RequisitionId,
                LineNo = item.LineNo,
                MaterialId = item.MaterialId,
                MaterialCode = item.MaterialCode,
                MaterialName = item.MaterialName,
                MaterialGrade = item.MaterialGrade,
                QuantityRequired = item.QuantityRequired,
                UOM = item.UOM,
                LengthRequiredMM = item.LengthRequiredMM,
                DiameterMM = item.DiameterMM,
                NumberOfPieces = item.NumberOfPieces,
                QuantityAllocated = item.QuantityAllocated,
                QuantityIssued = item.QuantityIssued,
                QuantityPending = item.QuantityPending,
                Status = item.Status,
                JobCardId = item.JobCardId,
                JobCardNo = item.JobCardNo,
                ProcessId = item.ProcessId,
                ProcessName = item.ProcessName,
                SelectedPieceIds = selectedPieceIds,
                Remarks = item.Remarks,
                AllocatedAt = item.AllocatedAt,
                IssuedAt = item.IssuedAt,
                CreatedAt = item.CreatedAt
            };
        }

        private static MaterialPieceResponse MapToPieceResponse(MaterialPiece piece)
        {
            return new MaterialPieceResponse
            {
                Id = piece.Id,
                MaterialId = piece.MaterialId,
                PieceNo = piece.PieceNo,
                OriginalLengthMM = piece.OriginalLengthMM,
                CurrentLengthMM = piece.CurrentLengthMM,
                OriginalWeightKG = piece.OriginalWeightKG,
                CurrentWeightKG = piece.CurrentWeightKG,
                Status = piece.Status,
                AllocatedToRequisitionId = piece.AllocatedToRequisitionId,
                IssuedToJobCardId = piece.IssuedToJobCardId,
                StorageLocation = piece.StorageLocation,
                BinNumber = piece.BinNumber,
                RackNumber = piece.RackNumber,
                GRNNo = piece.GRNNo,
                ReceivedDate = piece.ReceivedDate,
                SupplierBatchNo = piece.SupplierBatchNo,
                SupplierId = piece.SupplierId,
                UnitCost = piece.UnitCost,
                CreatedAt = piece.CreatedAt,
                UpdatedAt = piece.UpdatedAt
            };
        }

        private static MaterialIssueResponse MapToIssueResponse(MaterialIssue issue)
        {
            return new MaterialIssueResponse
            {
                Id = issue.Id,
                IssueNo = issue.IssueNo,
                IssueDate = issue.IssueDate,
                RequisitionId = issue.RequisitionId,
                JobCardNo = issue.JobCardNo,
                OrderNo = issue.OrderNo,
                MaterialName = issue.MaterialName,
                MaterialGrade = issue.MaterialGrade,
                TotalPieces = issue.TotalPieces,
                TotalIssuedLengthMM = issue.TotalIssuedLengthMM,
                TotalIssuedWeightKG = issue.TotalIssuedWeightKG,
                Status = issue.Status,
                IssuedById = issue.IssuedById,
                IssuedByName = issue.IssuedByName,
                ReceivedById = issue.ReceivedById,
                ReceivedByName = issue.ReceivedByName
            };
        }
    }

    // Helper Request DTOs for specific operations
    public class ApproveRequisitionRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string ApprovedBy { get; set; } = string.Empty;
    }

    public class RejectRequisitionRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string RejectedBy { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class UpdateStatusRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateItemSelectedPiecesRequest
    {
        public List<int> PieceIds { get; set; } = new();
    }
}
