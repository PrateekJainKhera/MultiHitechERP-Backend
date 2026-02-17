using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _service;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMaterialPieceRepository _pieceRepository;

        public WarehousesController(
            IWarehouseService service,
            IWarehouseRepository warehouseRepository,
            IMaterialPieceRepository pieceRepository)
        {
            _service = service;
            _warehouseRepository = warehouseRepository;
            _pieceRepository = pieceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get low stock status for all active warehouses.
        /// Returns all warehouses with their current available stock vs min thresholds.
        /// PiecesAlert = true when currentPieces &lt; minStockPieces (and minStockPieces &gt; 0).
        /// LengthAlert = true when currentLengthMM &lt; minStockLengthMM (and minStockLengthMM &gt; 0).
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _service.GetLowStockStatusAsync();
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        /// <summary>
        /// Relocate one or more pieces to a different warehouse.
        /// Supports single or bulk relocation.
        /// </summary>
        [HttpPost("relocate")]
        public async Task<IActionResult> RelocateInventory([FromBody] RelocateInventoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (request.PieceIds == null || request.PieceIds.Count == 0)
                    return BadRequest(ApiResponse<int>.ErrorResponse("No pieces selected for relocation"));

                // Validate destination warehouse exists
                var warehouse = await _warehouseRepository.GetByIdAsync(request.ToWarehouseId);
                if (warehouse == null)
                    return NotFound(ApiResponse<int>.ErrorResponse($"Destination warehouse {request.ToWarehouseId} not found"));

                var warehouseName = $"{warehouse.Name} - {warehouse.Rack} {warehouse.RackNo}";
                var updated = await _pieceRepository.RelocatePiecesAsync(
                    request.PieceIds,
                    request.ToWarehouseId,
                    warehouseName,
                    request.RelocatedBy ?? "Admin"
                );

                return Ok(ApiResponse<int>.SuccessResponse(updated,
                    $"{updated} piece(s) relocated to {warehouseName}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse($"Error relocating inventory: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all pieces currently in a specific warehouse.
        /// </summary>
        [HttpGet("{id}/pieces")]
        public async Task<IActionResult> GetPiecesByWarehouse(int id)
        {
            try
            {
                var pieces = await _pieceRepository.GetByWarehouseIdAsync(id);
                var responses = new List<MaterialPieceResponse>();
                foreach (var p in pieces)
                {
                    responses.Add(new MaterialPieceResponse
                    {
                        Id = p.Id, PieceNo = p.PieceNo, MaterialId = p.MaterialId,
                        MaterialCode = p.MaterialCode, MaterialName = p.MaterialName,
                        Grade = p.Grade, Diameter = p.Diameter,
                        OriginalLengthMM = p.OriginalLengthMM, CurrentLengthMM = p.CurrentLengthMM,
                        OriginalWeightKG = p.OriginalWeightKG, CurrentWeightKG = p.CurrentWeightKG,
                        Status = p.Status,
                        AllocatedToRequisitionId = p.AllocatedToRequisitionId,
                        IssuedToJobCardId = p.IssuedToJobCardId,
                        WarehouseId = p.WarehouseId, WarehouseName = p.WarehouseName,
                        StorageLocation = p.StorageLocation,
                        GRNId = p.GRNId, GRNNo = p.GRNNo, ReceivedDate = p.ReceivedDate,
                        IsWastage = p.IsWastage, WastageReason = p.WastageReason,
                        CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt
                    });
                }
                return Ok(ApiResponse<List<MaterialPieceResponse>>.SuccessResponse(responses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<MaterialPieceResponse>>.ErrorResponse($"Error: {ex.Message}"));
            }
        }
    }
}
