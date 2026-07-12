using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers.MIS
{
    /// <summary>
    /// MIS aggregates — real, countable numbers only.
    /// </summary>
    [ApiController]
    [Route("api/mis")]
    public class MISController : ControllerBase
    {
        private readonly IMISRepository _misRepository;

        public MISController(IMISRepository misRepository)
        {
            _misRepository = misRepository;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<ApiResponse<MISOverviewResponse>>> GetOverview()
        {
            try
            {
                var data = await _misRepository.GetOverviewAsync();
                return Ok(ApiResponse<MISOverviewResponse>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<MISOverviewResponse>.ErrorResponse($"Failed to load MIS overview: {ex.Message}"));
            }
        }

        /// <summary>Top 10 machine models by orders + distinct model list for search.</summary>
        [HttpGet("machine-models")]
        public async Task<ActionResult<ApiResponse<MachineModelsResponse>>> GetMachineModels()
        {
            try
            {
                return Ok(ApiResponse<MachineModelsResponse>.SuccessResponse(await _misRepository.GetMachineModelsAsync()));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<MachineModelsResponse>.ErrorResponse($"Failed to load machine models: {ex.Message}"));
            }
        }

        /// <summary>A model's roller/teeth variants + monthly & yearly order counts.</summary>
        [HttpGet("machine-models/detail")]
        public async Task<ActionResult<ApiResponse<MachineModelDetailResponse>>> GetMachineModelDetail([FromQuery] string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                return BadRequest(ApiResponse<MachineModelDetailResponse>.ErrorResponse("model is required"));
            try
            {
                return Ok(ApiResponse<MachineModelDetailResponse>.SuccessResponse(await _misRepository.GetMachineModelDetailAsync(model)));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<MachineModelDetailResponse>.ErrorResponse($"Failed to load model detail: {ex.Message}"));
            }
        }

        /// <summary>Customers who bought a specific Model (optionally a roller/teeth variant) + last order date.</summary>
        [HttpGet("machine-models/customers")]
        public async Task<ActionResult<ApiResponse<List<MachineModelCustomerRow>>>> GetMachineModelCustomers(
            [FromQuery] string model, [FromQuery] string? roller, [FromQuery] int? teeth)
        {
            if (string.IsNullOrWhiteSpace(model))
                return BadRequest(ApiResponse<List<MachineModelCustomerRow>>.ErrorResponse("model is required"));
            try
            {
                return Ok(ApiResponse<List<MachineModelCustomerRow>>.SuccessResponse(
                    await _misRepository.GetMachineModelCustomersAsync(model, roller, teeth)));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<MachineModelCustomerRow>>.ErrorResponse($"Failed to load customers: {ex.Message}"));
            }
        }
    }
}
