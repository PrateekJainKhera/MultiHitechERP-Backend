using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiHitechERP.API.Controllers.Inventory
{
    [ApiController]
    [Route("api/material-pieces")]
    public class MaterialPieceController : ControllerBase
    {
        private readonly IMaterialPieceService _materialPieceService;

        public MaterialPieceController(IMaterialPieceService materialPieceService)
        {
            _materialPieceService = materialPieceService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<MaterialPieceResponse>>>> GetAll()
        {
            try
            {
                var result = await _materialPieceService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MaterialPieceResponse>>> GetById(int id)
        {
            try
            {
                var result = await _materialPieceService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<MaterialPieceResponse>
                    {
                        Success = false,
                        Message = "Material piece not found"
                    });
                }

                return Ok(new ApiResponse<MaterialPieceResponse>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<MaterialPieceResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("by-material/{materialId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MaterialPieceResponse>>>> GetByMaterialId(int materialId)
        {
            try
            {
                var result = await _materialPieceService.GetByMaterialIdAsync(materialId);
                return Ok(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MaterialPieceResponse>>>> GetAvailable()
        {
            try
            {
                var result = await _materialPieceService.GetAvailablePiecesAsync();
                return Ok(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("wastage")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MaterialPieceResponse>>>> GetWastage()
        {
            try
            {
                var result = await _materialPieceService.GetWastagePiecesAsync();
                return Ok(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("by-grn/{grnId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MaterialPieceResponse>>>> GetByGRNId(int grnId)
        {
            try
            {
                var result = await _materialPieceService.GetByGRNIdAsync(grnId);
                return Ok(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<MaterialPieceResponse>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("stock/total/{materialId}")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalStock(int materialId)
        {
            try
            {
                var result = await _materialPieceService.GetTotalStockByMaterialIdAsync(materialId);
                return Ok(new ApiResponse<decimal>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<decimal>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("stock/available/{materialId}")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetAvailableStock(int materialId)
        {
            try
            {
                var result = await _materialPieceService.GetAvailableStockByMaterialIdAsync(materialId);
                return Ok(new ApiResponse<decimal>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<decimal>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPatch("{id}/adjust-length")]
        public async Task<ActionResult<ApiResponse<object>>> AdjustLength(int id, [FromBody] AdjustLengthRequest request)
        {
            try
            {
                var success = await _materialPieceService.AdjustLengthAsync(id, request.NewLengthMM, request.Remark, request.AdjustedBy);
                if (!success)
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Material piece not found" });

                return Ok(new ApiResponse<object> { Success = true, Message = "Length adjusted successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }
    }
}
