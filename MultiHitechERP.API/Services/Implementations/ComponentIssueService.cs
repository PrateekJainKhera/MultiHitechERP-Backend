using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ComponentIssueService : IComponentIssueService
    {
        private readonly IComponentIssueRepository _repo;
        private readonly IComponentRepository _componentRepo;
        private readonly IInventoryRepository _inventoryRepo;

        public ComponentIssueService(
            IComponentIssueRepository repo,
            IComponentRepository componentRepo,
            IInventoryRepository inventoryRepo)
        {
            _repo = repo;
            _componentRepo = componentRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<ApiResponse<ComponentIssueResponse>> CreateAsync(CreateComponentIssueRequest request)
        {
            try
            {
                if (request.IssuedQty <= 0)
                    return ApiResponse<ComponentIssueResponse>.ErrorResponse("Issued quantity must be greater than zero");

                if (string.IsNullOrWhiteSpace(request.RequestedBy))
                    return ApiResponse<ComponentIssueResponse>.ErrorResponse("Requested by is required");

                if (string.IsNullOrWhiteSpace(request.IssuedBy))
                    return ApiResponse<ComponentIssueResponse>.ErrorResponse("Issued by is required");

                // Load component details
                var component = await _componentRepo.GetByIdAsync(request.ComponentId);
                if (component == null)
                    return ApiResponse<ComponentIssueResponse>.ErrorResponse($"Component {request.ComponentId} not found");

                // Deduct from Inventory_Stock
                var deducted = await _inventoryRepo.DeductComponentStockAsync(
                    request.ComponentId,
                    request.IssuedQty,
                    request.IssuedBy);

                if (!deducted)
                    return ApiResponse<ComponentIssueResponse>.ErrorResponse(
                        $"Insufficient stock for {component.ComponentName}. Check available quantity.");

                // Generate issue number
                var issueNo = $"CI-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks % 100000:D5}";

                var issue = new ComponentIssue
                {
                    IssueNo       = issueNo,
                    IssueDate     = DateTime.UtcNow,
                    ComponentId   = request.ComponentId,
                    ComponentName = component.ComponentName,
                    PartNumber    = component.PartNumber,
                    Unit          = component.Unit ?? "Pcs",
                    IssuedQty     = request.IssuedQty,
                    RequestedBy   = request.RequestedBy.Trim(),
                    IssuedBy      = request.IssuedBy.Trim(),
                    Remarks       = request.Remarks?.Trim(),
                    CreatedAt     = DateTime.UtcNow,
                    CreatedBy     = request.CreatedBy ?? request.IssuedBy,
                };

                var id = await _repo.CreateAsync(issue);
                issue.Id = id;

                return ApiResponse<ComponentIssueResponse>.SuccessResponse(
                    MapToResponse(issue),
                    $"Component issued successfully. Issue No: {issueNo}");
            }
            catch (Exception ex)
            {
                return ApiResponse<ComponentIssueResponse>.ErrorResponse($"Failed to issue component: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ComponentIssueResponse>>> GetAllAsync()
        {
            try
            {
                var issues = await _repo.GetAllAsync();
                return ApiResponse<IEnumerable<ComponentIssueResponse>>.SuccessResponse(
                    issues.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentIssueResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ComponentIssueResponse>>> GetByComponentIdAsync(int componentId)
        {
            try
            {
                var issues = await _repo.GetByComponentIdAsync(componentId);
                return ApiResponse<IEnumerable<ComponentIssueResponse>>.SuccessResponse(
                    issues.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentIssueResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ComponentWithStockResponse>>> GetComponentsWithStockAsync()
        {
            try
            {
                var items = await _repo.GetComponentsWithStockAsync();
                return ApiResponse<IEnumerable<ComponentWithStockResponse>>.SuccessResponse(items);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ComponentWithStockResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static ComponentIssueResponse MapToResponse(ComponentIssue i) => new()
        {
            Id            = i.Id,
            IssueNo       = i.IssueNo,
            IssueDate     = i.IssueDate.ToString("yyyy-MM-dd"),
            ComponentId   = i.ComponentId,
            ComponentName = i.ComponentName,
            PartNumber    = i.PartNumber,
            Unit          = i.Unit,
            IssuedQty     = i.IssuedQty,
            RequestedBy   = i.RequestedBy,
            IssuedBy      = i.IssuedBy,
            Remarks       = i.Remarks,
            CreatedAt     = i.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
        };
    }
}
