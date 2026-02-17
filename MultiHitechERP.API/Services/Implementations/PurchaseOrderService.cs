using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IVendorRepository _vendorRepo;
        private readonly IPurchaseRequestRepository _prRepo;

        public PurchaseOrderService(
            IPurchaseOrderRepository poRepo,
            IVendorRepository vendorRepo,
            IPurchaseRequestRepository prRepo)
        {
            _poRepo = poRepo;
            _vendorRepo = vendorRepo;
            _prRepo = prRepo;
        }

        public async Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> GetAllAsync()
        {
            var pos = await _poRepo.GetAllAsync();
            var responses = await MapManyAsync(pos);
            return ApiResponse<IEnumerable<PurchaseOrderResponse>>.SuccessResponse(responses);
        }

        public async Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> GetByVendorAsync(int vendorId)
        {
            var pos = await _poRepo.GetByVendorAsync(vendorId);
            var responses = await MapManyAsync(pos);
            return ApiResponse<IEnumerable<PurchaseOrderResponse>>.SuccessResponse(responses);
        }

        public async Task<ApiResponse<IEnumerable<PurchaseOrderResponse>>> GetByPurchaseRequestAsync(int prId)
        {
            var pos = await _poRepo.GetByPurchaseRequestAsync(prId);
            var responses = await MapManyAsync(pos);
            return ApiResponse<IEnumerable<PurchaseOrderResponse>>.SuccessResponse(responses);
        }

        public async Task<ApiResponse<PurchaseOrderResponse>> GetByIdAsync(int id)
        {
            var po = await _poRepo.GetByIdAsync(id);
            if (po == null)
                return ApiResponse<PurchaseOrderResponse>.ErrorResponse("Purchase Order not found");
            return ApiResponse<PurchaseOrderResponse>.SuccessResponse(await MapAsync(po));
        }

        public async Task<ApiResponse<bool>> SendAsync(int id)
        {
            var po = await _poRepo.GetByIdAsync(id);
            if (po == null) return ApiResponse<bool>.ErrorResponse("Purchase Order not found");
            if (po.Status != "Draft") return ApiResponse<bool>.ErrorResponse("Only Draft POs can be sent");

            await _poRepo.UpdateStatusAsync(id, "Sent");
            return ApiResponse<bool>.SuccessResponse(true, "Purchase Order marked as Sent");
        }

        public async Task<ApiResponse<bool>> CancelAsync(int id)
        {
            var po = await _poRepo.GetByIdAsync(id);
            if (po == null) return ApiResponse<bool>.ErrorResponse("Purchase Order not found");
            if (po.Status == "Cancelled") return ApiResponse<bool>.ErrorResponse("PO is already cancelled");

            await _poRepo.UpdateStatusAsync(id, "Cancelled");
            return ApiResponse<bool>.SuccessResponse(true, "Purchase Order cancelled");
        }

        private async Task<PurchaseOrderResponse> MapAsync(Models.Procurement.PurchaseOrder po)
        {
            var vendor = await _vendorRepo.GetByIdAsync(po.VendorId);
            string? prNumber = null;
            if (po.PurchaseRequestId.HasValue)
            {
                var pr = await _prRepo.GetByIdAsync(po.PurchaseRequestId.Value);
                prNumber = pr?.PRNumber;
            }

            return new PurchaseOrderResponse
            {
                Id = po.Id,
                PONumber = po.PONumber,
                PurchaseRequestId = po.PurchaseRequestId,
                PRNumber = prNumber,
                VendorId = po.VendorId,
                VendorName = vendor?.VendorName ?? string.Empty,
                VendorCode = vendor?.VendorCode ?? string.Empty,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                Notes = po.Notes,
                CreatedAt = po.CreatedAt,
                CreatedBy = po.CreatedBy,
                Items = po.Items.Select(i => new PurchaseOrderItemResponse
                {
                    Id = i.Id,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseRequestItemId = i.PurchaseRequestItemId,
                    ItemType = i.ItemType,
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    ItemCode = i.ItemCode,
                    Unit = i.Unit,
                    OrderedQty = i.OrderedQty,
                    UnitCost = i.UnitCost,
                    TotalCost = i.TotalCost,
                }).ToList()
            };
        }

        private async Task<IEnumerable<PurchaseOrderResponse>> MapManyAsync(IEnumerable<Models.Procurement.PurchaseOrder> pos)
        {
            var results = new List<PurchaseOrderResponse>();
            foreach (var po in pos)
                results.Add(await MapAsync(po));
            return results;
        }
    }
}
