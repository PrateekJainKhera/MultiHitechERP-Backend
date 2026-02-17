using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;
using MultiHitechERP.API.Models.Procurement;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IPurchaseRequestRepository _prRepo;
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IVendorRepository _vendorRepo;

        public PurchaseRequestService(
            IPurchaseRequestRepository prRepo,
            IPurchaseOrderRepository poRepo,
            IVendorRepository vendorRepo)
        {
            _prRepo = prRepo;
            _poRepo = poRepo;
            _vendorRepo = vendorRepo;
        }

        public async Task<ApiResponse<IEnumerable<PurchaseRequestResponse>>> GetAllAsync()
        {
            var prs = await _prRepo.GetAllAsync();
            return ApiResponse<IEnumerable<PurchaseRequestResponse>>.SuccessResponse(
                await MapManyAsync(prs));
        }

        public async Task<ApiResponse<IEnumerable<PurchaseRequestResponse>>> GetByStatusAsync(string status)
        {
            var prs = await _prRepo.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<PurchaseRequestResponse>>.SuccessResponse(
                await MapManyAsync(prs));
        }

        public async Task<ApiResponse<IEnumerable<PurchaseRequestResponse>>> GetByItemTypeAsync(string itemType)
        {
            var prs = await _prRepo.GetByItemTypeAsync(itemType);
            return ApiResponse<IEnumerable<PurchaseRequestResponse>>.SuccessResponse(
                await MapManyAsync(prs));
        }

        public async Task<ApiResponse<PurchaseRequestResponse>> GetByIdAsync(int id)
        {
            var pr = await _prRepo.GetByIdAsync(id);
            if (pr == null) return ApiResponse<PurchaseRequestResponse>.ErrorResponse("Purchase Request not found");
            return ApiResponse<PurchaseRequestResponse>.SuccessResponse(await MapAsync(pr));
        }

        public async Task<ApiResponse<int>> CreateAsync(CreatePurchaseRequestRequest request)
        {
            if (request.Items.Count == 0)
                return ApiResponse<int>.ErrorResponse("At least one item is required");

            var seq = await _prRepo.GetNextSequenceNumberAsync();
            var month = DateTime.UtcNow.ToString("yyyyMM");
            var prNumber = $"PR-{month}-{seq:D3}";

            var pr = new PurchaseRequest
            {
                PRNumber = prNumber,
                ItemType = request.ItemType,
                RequestedBy = request.RequestedBy,
                RequestDate = DateTime.UtcNow,
                Notes = request.Notes,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                Items = request.Items.Select(i => new PurchaseRequestItem
                {
                    ItemType = i.ItemType,
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    ItemCode = i.ItemCode,
                    Unit = i.Unit,
                    RequestedQty = i.RequestedQty,
                    Status = "Pending",
                    Notes = i.Notes,
                    CuttingList = i.CuttingList.Select(c => new PRItemCuttingListEntry
                    {
                        LengthMeter = c.LengthMeter,
                        Pieces = c.Pieces,
                        Notes = c.Notes,
                    }).ToList()
                }).ToList()
            };

            var id = await _prRepo.InsertAsync(pr);
            return ApiResponse<int>.SuccessResponse(id, $"Purchase Request {prNumber} created");
        }

        public async Task<ApiResponse<bool>> SubmitAsync(int id)
        {
            var pr = await _prRepo.GetByIdAsync(id);
            if (pr == null) return ApiResponse<bool>.ErrorResponse("Purchase Request not found");
            if (pr.Status != "Draft") return ApiResponse<bool>.ErrorResponse("Only Draft PRs can be submitted");

            await _prRepo.UpdateStatusAsync(id, "Submitted");
            return ApiResponse<bool>.SuccessResponse(true, "Purchase Request submitted for approval");
        }

        public async Task<ApiResponse<bool>> StartReviewAsync(int id)
        {
            var pr = await _prRepo.GetByIdAsync(id);
            if (pr == null) return ApiResponse<bool>.ErrorResponse("Purchase Request not found");
            if (pr.Status != "Submitted") return ApiResponse<bool>.ErrorResponse("Only Submitted PRs can be moved to review");

            await _prRepo.UpdateStatusAsync(id, "UnderApproval");
            return ApiResponse<bool>.SuccessResponse(true, "Purchase Request is now under approval");
        }

        public async Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> ApproveAsync(int id, ApprovePurchaseRequestRequest request)
        {
            var pr = await _prRepo.GetByIdAsync(id);
            if (pr == null) return ApiResponse<IEnumerable<PurchaseOrderResponse>>.ErrorResponse("Purchase Request not found");
            if (pr.Status != "UnderApproval" && pr.Status != "Submitted")
                return ApiResponse<IEnumerable<PurchaseOrderResponse>>.ErrorResponse("PR must be in Submitted or UnderApproval status");

            // Update each item
            foreach (var approvalItem in request.Items)
            {
                var prItem = pr.Items.FirstOrDefault(i => i.Id == approvalItem.ItemId);
                if (prItem == null) continue;

                prItem.Status = approvalItem.Status;
                prItem.ApprovedQty = approvalItem.Status == "Approved" ? (approvalItem.ApprovedQty ?? prItem.RequestedQty) : null;
                prItem.VendorId = approvalItem.VendorId;
                prItem.EstimatedUnitCost = approvalItem.EstimatedUnitCost;
                prItem.Notes = approvalItem.Notes;
                await _prRepo.UpdateItemAsync(prItem);
            }

            // Check if any items are approved
            var approvedItems = pr.Items.Where(i => i.Status == "Approved" && i.VendorId.HasValue).ToList();
            if (!approvedItems.Any())
            {
                // All rejected
                await _prRepo.UpdateStatusAsync(id, "Rejected", null, "All items rejected");
                return ApiResponse<IEnumerable<PurchaseOrderResponse>>.SuccessResponse(
                    new List<PurchaseOrderResponse>(), "PR rejected - no items approved");
            }

            // Generate POs grouped by vendor
            var vendorGroups = approvedItems.GroupBy(i => i.VendorId!.Value);
            var generatedPOs = new List<PurchaseOrderResponse>();

            foreach (var group in vendorGroups)
            {
                var vendor = await _vendorRepo.GetByIdAsync(group.Key);
                if (vendor == null) continue;

                var poSeq = await _poRepo.GetNextSequenceNumberAsync();
                var month = DateTime.UtcNow.ToString("yyyyMM");
                var poNumber = $"PO-{month}-{poSeq:D3}";

                var totalAmount = group.Sum(i =>
                    (i.ApprovedQty ?? i.RequestedQty) * (i.EstimatedUnitCost ?? 0));

                var po = new PurchaseOrder
                {
                    PONumber = poNumber,
                    PurchaseRequestId = id,
                    VendorId = group.Key,
                    Status = "Draft",
                    TotalAmount = totalAmount > 0 ? totalAmount : null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.ApprovedBy,
                    Items = group.Select(i => new PurchaseOrderItem
                    {
                        PurchaseRequestItemId = i.Id,
                        ItemType = i.ItemType,
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        ItemCode = i.ItemCode,
                        Unit = i.Unit,
                        OrderedQty = i.ApprovedQty ?? i.RequestedQty,
                        UnitCost = i.EstimatedUnitCost,
                        TotalCost = i.EstimatedUnitCost.HasValue
                            ? (i.ApprovedQty ?? i.RequestedQty) * i.EstimatedUnitCost.Value
                            : null,
                    }).ToList()
                };

                var poId = await _poRepo.InsertAsync(po);
                po.Id = poId;

                generatedPOs.Add(new PurchaseOrderResponse
                {
                    Id = poId,
                    PONumber = poNumber,
                    PurchaseRequestId = id,
                    PRNumber = pr.PRNumber,
                    VendorId = group.Key,
                    VendorName = vendor.VendorName,
                    VendorCode = vendor.VendorCode,
                    Status = "Draft",
                    TotalAmount = po.TotalAmount,
                    CreatedAt = po.CreatedAt,
                    Items = po.Items.Select(i => new PurchaseOrderItemResponse
                    {
                        ItemType = i.ItemType,
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        ItemCode = i.ItemCode,
                        Unit = i.Unit,
                        OrderedQty = i.OrderedQty,
                        UnitCost = i.UnitCost,
                        TotalCost = i.TotalCost,
                    }).ToList()
                });
            }

            await _prRepo.UpdateStatusAsync(id, "POGenerated", request.ApprovedBy);
            return ApiResponse<IEnumerable<PurchaseOrderResponse>>.SuccessResponse(
                generatedPOs, $"PR approved. {generatedPOs.Count} PO(s) generated.");
        }

        public async Task<ApiResponse<bool>> RejectAsync(int id, string rejectionReason, string rejectedBy)
        {
            var pr = await _prRepo.GetByIdAsync(id);
            if (pr == null) return ApiResponse<bool>.ErrorResponse("Purchase Request not found");

            await _prRepo.UpdateStatusAsync(id, "Rejected", null, rejectionReason);
            return ApiResponse<bool>.SuccessResponse(true, "Purchase Request rejected");
        }

        public async Task<ApiResponse<bool>> AddItemAsync(int prId, CreatePurchaseRequestItemRequest item)
        {
            var pr = await _prRepo.GetByIdAsync(prId);
            if (pr == null) return ApiResponse<bool>.ErrorResponse("Purchase Request not found");
            if (pr.Status != "Draft")
                return ApiResponse<bool>.ErrorResponse("Items can only be added to Draft PRs");

            var prItem = new PurchaseRequestItem
            {
                PurchaseRequestId = prId,
                ItemType = item.ItemType,
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                ItemCode = item.ItemCode,
                Unit = item.Unit,
                RequestedQty = item.RequestedQty,
                Status = "Pending",
                Notes = item.Notes,
            };
            await _prRepo.InsertItemAsync(prItem);
            return ApiResponse<bool>.SuccessResponse(true, "Item added");
        }

        public async Task<ApiResponse<bool>> RemoveItemAsync(int prId, int itemId)
        {
            var pr = await _prRepo.GetByIdAsync(prId);
            if (pr == null) return ApiResponse<bool>.ErrorResponse("Purchase Request not found");
            if (pr.Status != "Draft" && pr.Status != "UnderApproval")
                return ApiResponse<bool>.ErrorResponse("Items can only be removed from Draft or UnderApproval PRs");

            await _prRepo.DeleteItemAsync(itemId);
            return ApiResponse<bool>.SuccessResponse(true, "Item removed");
        }

        private async Task<PurchaseRequestResponse> MapAsync(PurchaseRequest pr)
        {
            // Load vendor names for items
            var vendorIds = pr.Items.Where(i => i.VendorId.HasValue).Select(i => i.VendorId!.Value).Distinct();
            var vendorNames = new Dictionary<int, string>();
            foreach (var vid in vendorIds)
            {
                var v = await _vendorRepo.GetByIdAsync(vid);
                if (v != null) vendorNames[vid] = v.VendorName;
            }

            return new PurchaseRequestResponse
            {
                Id = pr.Id,
                PRNumber = pr.PRNumber,
                ItemType = pr.ItemType,
                RequestedBy = pr.RequestedBy,
                RequestDate = pr.RequestDate,
                Notes = pr.Notes,
                Status = pr.Status,
                RejectionReason = pr.RejectionReason,
                ApprovedBy = pr.ApprovedBy,
                ApprovedAt = pr.ApprovedAt,
                CreatedAt = pr.CreatedAt,
                Items = pr.Items.Select(i => new PurchaseRequestItemResponse
                {
                    Id = i.Id,
                    PurchaseRequestId = i.PurchaseRequestId,
                    ItemType = i.ItemType,
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    ItemCode = i.ItemCode,
                    Unit = i.Unit,
                    RequestedQty = i.RequestedQty,
                    ApprovedQty = i.ApprovedQty,
                    VendorId = i.VendorId,
                    VendorName = i.VendorId.HasValue && vendorNames.TryGetValue(i.VendorId.Value, out var vn) ? vn : null,
                    EstimatedUnitCost = i.EstimatedUnitCost,
                    Status = i.Status,
                    Notes = i.Notes,
                    CuttingList = i.CuttingList.Select(c => new CuttingListItemResponse
                    {
                        Id = c.Id,
                        PRItemId = c.PRItemId,
                        LengthMeter = c.LengthMeter,
                        Pieces = c.Pieces,
                        TotalLengthMeter = c.TotalLengthMeter,
                        Notes = c.Notes,
                    }).ToList()
                }).ToList()
            };
        }

        private async Task<IEnumerable<PurchaseRequestResponse>> MapManyAsync(IEnumerable<PurchaseRequest> prs)
        {
            var results = new List<PurchaseRequestResponse>();
            foreach (var pr in prs)
                results.Add(await MapAsync(pr));
            return results;
        }
    }
}
