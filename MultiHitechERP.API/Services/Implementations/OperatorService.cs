using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class OperatorService : IOperatorService
    {
        private readonly IOperatorRepository _operatorRepository;

        public OperatorService(IOperatorRepository operatorRepository)
        {
            _operatorRepository = operatorRepository;
        }

        public async Task<ApiResponse<OperatorResponse>> GetByIdAsync(int id)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByIdAsync(id);
                if (operatorEntity == null)
                    return ApiResponse<OperatorResponse>.ErrorResponse($"Operator with ID {id} not found");

                return ApiResponse<OperatorResponse>.SuccessResponse(MapToResponse(operatorEntity));
            }
            catch (Exception ex)
            {
                return ApiResponse<OperatorResponse>.ErrorResponse($"Error retrieving operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<OperatorResponse>> GetByOperatorCodeAsync(string operatorCode)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByOperatorCodeAsync(operatorCode);
                if (operatorEntity == null)
                    return ApiResponse<OperatorResponse>.ErrorResponse($"Operator {operatorCode} not found");

                return ApiResponse<OperatorResponse>.SuccessResponse(MapToResponse(operatorEntity));
            }
            catch (Exception ex)
            {
                return ApiResponse<OperatorResponse>.ErrorResponse($"Error retrieving operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetAllAsync()
        {
            try
            {
                var operators = await _operatorRepository.GetAllAsync();
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetActiveOperatorsAsync()
        {
            try
            {
                var operators = await _operatorRepository.GetActiveOperatorsAsync();
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving active operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetAvailableOperatorsAsync()
        {
            try
            {
                var operators = await _operatorRepository.GetAvailableOperatorsAsync();
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving available operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetByDepartmentAsync(string department)
        {
            try
            {
                var operators = await _operatorRepository.GetByDepartmentAsync(department);
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetByShiftAsync(string shift)
        {
            try
            {
                var operators = await _operatorRepository.GetByShiftAsync(shift);
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetBySkillLevelAsync(string skillLevel)
        {
            try
            {
                var operators = await _operatorRepository.GetBySkillLevelAsync(skillLevel);
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<OperatorResponse>>> GetByMachineExpertiseAsync(int machineId)
        {
            try
            {
                var operators = await _operatorRepository.GetByMachineExpertiseAsync(machineId);
                var responses = operators.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<OperatorResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<OperatorResponse>>.ErrorResponse($"Error retrieving operators: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateOperatorAsync(CreateOperatorRequest request)
        {
            try
            {
                // Validation
                var exists = await _operatorRepository.ExistsAsync(request.OperatorCode);
                if (exists)
                    return ApiResponse<int>.ErrorResponse($"Operator code '{request.OperatorCode}' already exists");

                if (string.IsNullOrWhiteSpace(request.OperatorName))
                    return ApiResponse<int>.ErrorResponse("Operator name is required");

                // Map to entity
                var operatorEntity = new Operator
                {
                    OperatorCode = request.OperatorCode.Trim().ToUpper(),
                    OperatorName = request.OperatorName.Trim(),
                    Email = request.Email?.Trim(),
                    Phone = request.Phone?.Trim(),
                    Mobile = request.Mobile?.Trim(),
                    EmployeeId = request.EmployeeId?.Trim(),
                    JoiningDate = request.JoiningDate,
                    Designation = request.Designation?.Trim(),
                    Department = request.Department?.Trim(),
                    ShopFloor = request.ShopFloor?.Trim(),
                    SkillLevel = request.SkillLevel?.Trim(),
                    Specialization = request.Specialization?.Trim(),
                    CertificationDetails = request.CertificationDetails?.Trim(),
                    MachineExpertise = request.MachineExpertise,
                    ProcessExpertise = request.ProcessExpertise,
                    Shift = request.Shift?.Trim(),
                    WorkingHours = request.WorkingHours?.Trim(),
                    EfficiencyRating = request.EfficiencyRating,
                    QualityRating = request.QualityRating,
                    HourlyRate = request.HourlyRate,
                    MonthlySalary = request.MonthlySalary,
                    IsActive = true,
                    Status = "Active",
                    IsAvailable = true,
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var operatorId = await _operatorRepository.InsertAsync(operatorEntity);
                return ApiResponse<int>.SuccessResponse(operatorId, $"Operator '{request.OperatorCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateOperatorAsync(UpdateOperatorRequest request)
        {
            try
            {
                var existingOperator = await _operatorRepository.GetByIdAsync(request.Id);
                if (existingOperator == null)
                    return ApiResponse<bool>.ErrorResponse("Operator not found");

                // Check if operator code is being changed and if new code already exists
                if (existingOperator.OperatorCode != request.OperatorCode)
                {
                    var exists = await _operatorRepository.ExistsAsync(request.OperatorCode);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Operator code '{request.OperatorCode}' already exists");
                }

                // Update fields
                existingOperator.OperatorCode = request.OperatorCode.Trim().ToUpper();
                existingOperator.OperatorName = request.OperatorName.Trim();
                existingOperator.Email = request.Email?.Trim();
                existingOperator.Phone = request.Phone?.Trim();
                existingOperator.Mobile = request.Mobile?.Trim();
                existingOperator.EmployeeId = request.EmployeeId?.Trim();
                existingOperator.JoiningDate = request.JoiningDate;
                existingOperator.Designation = request.Designation?.Trim();
                existingOperator.Department = request.Department?.Trim();
                existingOperator.ShopFloor = request.ShopFloor?.Trim();
                existingOperator.SkillLevel = request.SkillLevel?.Trim();
                existingOperator.Specialization = request.Specialization?.Trim();
                existingOperator.CertificationDetails = request.CertificationDetails?.Trim();
                existingOperator.MachineExpertise = request.MachineExpertise;
                existingOperator.ProcessExpertise = request.ProcessExpertise;
                existingOperator.Shift = request.Shift?.Trim();
                existingOperator.WorkingHours = request.WorkingHours?.Trim();
                existingOperator.EfficiencyRating = request.EfficiencyRating;
                existingOperator.QualityRating = request.QualityRating;
                existingOperator.HourlyRate = request.HourlyRate;
                existingOperator.MonthlySalary = request.MonthlySalary;
                existingOperator.IsActive = request.IsActive;
                existingOperator.Status = request.Status;
                existingOperator.IsAvailable = request.IsAvailable;
                existingOperator.CurrentJobCardId = request.CurrentJobCardId;
                existingOperator.CurrentJobCardNo = request.CurrentJobCardNo;
                existingOperator.CurrentMachineId = request.CurrentMachineId;
                existingOperator.Remarks = request.Remarks?.Trim();
                existingOperator.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingOperator.UpdatedAt = DateTime.UtcNow;

                var success = await _operatorRepository.UpdateAsync(existingOperator);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update operator");

                return ApiResponse<bool>.SuccessResponse(true, "Operator updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteOperatorAsync(int id)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByIdAsync(id);
                if (operatorEntity == null)
                    return ApiResponse<bool>.ErrorResponse("Operator not found");

                var success = await _operatorRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete operator");

                return ApiResponse<bool>.SuccessResponse(true, "Operator deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByIdAsync(id);
                if (operatorEntity == null)
                    return ApiResponse<bool>.ErrorResponse("Operator not found");

                var success = await _operatorRepository.UpdateStatusAsync(id, status);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update operator status");

                return ApiResponse<bool>.SuccessResponse(true, $"Operator status updated to '{status}'");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating operator status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAvailabilityAsync(int id, bool isAvailable)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByIdAsync(id);
                if (operatorEntity == null)
                    return ApiResponse<bool>.ErrorResponse("Operator not found");

                var success = await _operatorRepository.UpdateAvailabilityAsync(id, isAvailable);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update operator availability");

                return ApiResponse<bool>.SuccessResponse(true, $"Operator availability updated to {(isAvailable ? "Available" : "Unavailable")}");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating operator availability: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AssignToJobCardAsync(int id, int jobCardId, string jobCardNo, int? machineId)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByIdAsync(id);
                if (operatorEntity == null)
                    return ApiResponse<bool>.ErrorResponse("Operator not found");

                if (!operatorEntity.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Operator is not active");

                if (!operatorEntity.IsAvailable)
                    return ApiResponse<bool>.ErrorResponse($"Operator is already assigned to job card {operatorEntity.CurrentJobCardNo}");

                var success = await _operatorRepository.AssignToJobCardAsync(id, jobCardId, jobCardNo, machineId);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to assign operator to job card");

                return ApiResponse<bool>.SuccessResponse(true, $"Operator assigned to job card {jobCardNo}");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error assigning operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ReleaseFromJobCardAsync(int id)
        {
            try
            {
                var operatorEntity = await _operatorRepository.GetByIdAsync(id);
                if (operatorEntity == null)
                    return ApiResponse<bool>.ErrorResponse("Operator not found");

                if (operatorEntity.CurrentJobCardId == null)
                    return ApiResponse<bool>.ErrorResponse("Operator is not assigned to any job card");

                var success = await _operatorRepository.ReleaseFromJobCardAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to release operator from job card");

                return ApiResponse<bool>.SuccessResponse(true, "Operator released from job card");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error releasing operator: {ex.Message}");
            }
        }

        private static OperatorResponse MapToResponse(Operator operatorEntity)
        {
            return new OperatorResponse
            {
                Id = operatorEntity.Id,
                OperatorCode = operatorEntity.OperatorCode,
                OperatorName = operatorEntity.OperatorName,
                Email = operatorEntity.Email,
                Phone = operatorEntity.Phone,
                Mobile = operatorEntity.Mobile,
                EmployeeId = operatorEntity.EmployeeId,
                JoiningDate = operatorEntity.JoiningDate,
                Designation = operatorEntity.Designation,
                Department = operatorEntity.Department,
                ShopFloor = operatorEntity.ShopFloor,
                SkillLevel = operatorEntity.SkillLevel,
                Specialization = operatorEntity.Specialization,
                CertificationDetails = operatorEntity.CertificationDetails,
                MachineExpertise = operatorEntity.MachineExpertise,
                ProcessExpertise = operatorEntity.ProcessExpertise,
                Shift = operatorEntity.Shift,
                WorkingHours = operatorEntity.WorkingHours,
                EfficiencyRating = operatorEntity.EfficiencyRating,
                QualityRating = operatorEntity.QualityRating,
                HourlyRate = operatorEntity.HourlyRate,
                MonthlySalary = operatorEntity.MonthlySalary,
                IsActive = operatorEntity.IsActive,
                Status = operatorEntity.Status,
                IsAvailable = operatorEntity.IsAvailable,
                CurrentJobCardId = operatorEntity.CurrentJobCardId,
                CurrentJobCardNo = operatorEntity.CurrentJobCardNo,
                CurrentMachineId = operatorEntity.CurrentMachineId,
                Remarks = operatorEntity.Remarks,
                CreatedAt = operatorEntity.CreatedAt,
                CreatedBy = operatorEntity.CreatedBy,
                UpdatedAt = operatorEntity.UpdatedAt,
                UpdatedBy = operatorEntity.UpdatedBy
            };
        }
    }
}
