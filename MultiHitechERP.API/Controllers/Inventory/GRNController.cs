using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiHitechERP.API.Controllers.Inventory
{
    [ApiController]
    [Route("api/grn")]
    public class GRNController : ControllerBase
    {
        private readonly IGRNService _grnService;

        public GRNController(IGRNService grnService)
        {
            _grnService = grnService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<GRNResponse>>> Create([FromBody] CreateGRNRequest request)
        {
            try
            {
                var result = await _grnService.CreateGRNAsync(request);
                var message = result.RequiresApproval
                    ? "GRN submitted for approval — variance exceeds 5%. Material will be added to inventory after admin approval."
                    : "GRN created successfully and material pieces added to inventory";
                return Ok(new ApiResponse<GRNResponse> { Success = true, Message = message, Data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<GRNResponse> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("pending-approval")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GRNResponse>>>> GetPendingApproval()
        {
            try
            {
                var result = await _grnService.GetPendingApprovalAsync();
                return Ok(new ApiResponse<IEnumerable<GRNResponse>> { Success = true, Data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<GRNResponse>> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ApiResponse<GRNResponse>>> Approve(int id, [FromBody] GRNApprovalRequest request)
        {
            try
            {
                var result = await _grnService.ApproveGRNAsync(id, request.ActionBy, request.Notes);
                return Ok(new ApiResponse<GRNResponse> { Success = true, Message = "GRN approved — material pieces added to inventory", Data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<GRNResponse> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult<ApiResponse<GRNResponse>>> Reject(int id, [FromBody] GRNApprovalRequest request)
        {
            try
            {
                var result = await _grnService.RejectGRNAsync(id, request.ActionBy, request.Notes ?? "");
                return Ok(new ApiResponse<GRNResponse> { Success = true, Message = "GRN rejected", Data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<GRNResponse> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GRNResponse>>>> GetAll()
        {
            try
            {
                var result = await _grnService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<GRNResponse>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<GRNResponse>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GRNResponse>>> GetById(int id)
        {
            try
            {
                var result = await _grnService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<GRNResponse>
                    {
                        Success = false,
                        Message = "GRN not found"
                    });
                }

                return Ok(new ApiResponse<GRNResponse>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<GRNResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}/with-lines")]
        public async Task<ActionResult<ApiResponse<GRNResponse>>> GetWithLines(int id)
        {
            try
            {
                var result = await _grnService.GetGRNWithLinesAsync(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<GRNResponse>
                    {
                        Success = false,
                        Message = "GRN not found"
                    });
                }

                return Ok(new ApiResponse<GRNResponse>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<GRNResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("by-grn-no/{grnNo}")]
        public async Task<ActionResult<ApiResponse<GRNResponse>>> GetByGRNNo(string grnNo)
        {
            try
            {
                var result = await _grnService.GetByGRNNoAsync(grnNo);
                if (result == null)
                {
                    return NotFound(new ApiResponse<GRNResponse>
                    {
                        Success = false,
                        Message = "GRN not found"
                    });
                }

                return Ok(new ApiResponse<GRNResponse>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<GRNResponse>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var result = await _grnService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "GRN not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "GRN deleted successfully",
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
