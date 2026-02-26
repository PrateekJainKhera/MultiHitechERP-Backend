using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class OSPTrackingService : IOSPTrackingService
    {
        private readonly IOSPTrackingRepository _ospRepo;
        private readonly IJobCardRepository _jobCardRepo;

        public OSPTrackingService(
            IOSPTrackingRepository ospRepo,
            IJobCardRepository jobCardRepo)
        {
            _ospRepo = ospRepo;
            _jobCardRepo = jobCardRepo;
        }

        public async Task<ApiResponse<IEnumerable<OSPTracking>>> GetAllAsync()
        {
            var entries = await _ospRepo.GetAllAsync();
            return ApiResponse<IEnumerable<OSPTracking>>.SuccessResponse(entries);
        }

        public async Task<ApiResponse<IEnumerable<OSPJobCardOption>>> GetAvailableJobCardsAsync()
        {
            var options = await _ospRepo.GetAvailableJobCardsAsync();
            return ApiResponse<IEnumerable<OSPJobCardOption>>.SuccessResponse(options);
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateOSPTrackingRequest request)
        {
            var jc = await _jobCardRepo.GetByIdAsync(request.JobCardId);
            if (jc == null)
                return ApiResponse<int>.ErrorResponse("Job card not found");

            if (request.ExpectedReturnDate <= request.SentDate)
                return ApiResponse<int>.ErrorResponse("Expected return date must be after sent date");

            var entry = new OSPTracking
            {
                JobCardId          = jc.Id,
                JobCardNo          = jc.JobCardNo,
                OrderId            = jc.OrderId,
                OrderNo            = jc.OrderNo,
                OrderItemId        = jc.OrderItemId,
                ItemSequence       = jc.ItemSequence,
                ChildPartName      = jc.ChildPartName,
                ProcessName        = jc.ProcessName,
                VendorId           = request.VendorId,
                Quantity           = request.Quantity,
                SentDate           = request.SentDate.Date,
                ExpectedReturnDate = request.ExpectedReturnDate.Date,
                Status             = "Sent",
                Notes              = request.Notes,
                CreatedBy          = request.CreatedBy ?? "System",
            };

            var id = await _ospRepo.InsertAsync(entry);
            return ApiResponse<int>.SuccessResponse(id, "OSP entry created successfully");
        }

        public async Task<ApiResponse<IEnumerable<int>>> BatchCreateAsync(BatchCreateOSPRequest request)
        {
            if (request.JobCardIds == null || request.JobCardIds.Count == 0)
                return ApiResponse<IEnumerable<int>>.ErrorResponse("No job cards selected");

            if (request.ExpectedReturnDate <= request.SentDate)
                return ApiResponse<IEnumerable<int>>.ErrorResponse("Expected return date must be after sent date");

            var entries = new List<OSPTracking>();

            foreach (var jobCardId in request.JobCardIds)
            {
                var jc = await _jobCardRepo.GetByIdAsync(jobCardId);
                if (jc == null)
                    return ApiResponse<IEnumerable<int>>.ErrorResponse($"Job card {jobCardId} not found");

                entries.Add(new OSPTracking
                {
                    JobCardId          = jc.Id,
                    JobCardNo          = jc.JobCardNo,
                    OrderId            = jc.OrderId,
                    OrderNo            = jc.OrderNo,
                    OrderItemId        = jc.OrderItemId,
                    ItemSequence       = jc.ItemSequence,
                    ChildPartName      = jc.ChildPartName,
                    ProcessName        = jc.ProcessName,
                    VendorId           = request.VendorId,
                    Quantity           = jc.Quantity,
                    SentDate           = request.SentDate.Date,
                    ExpectedReturnDate = request.ExpectedReturnDate.Date,
                    Status             = "Sent",
                    Notes              = request.Notes,
                    CreatedBy          = request.CreatedBy ?? "System",
                });
            }

            var ids = await _ospRepo.BatchInsertAsync(entries);
            return ApiResponse<IEnumerable<int>>.SuccessResponse(ids,
                $"{ids.Count()} OSP entries created successfully");
        }

        public async Task<ApiResponse<bool>> MarkReceivedAsync(int id, ReceiveOSPRequest request)
        {
            var entry = await _ospRepo.GetByIdAsync(id);
            if (entry == null)
                return ApiResponse<bool>.ErrorResponse("OSP entry not found");

            if (entry.Status == "Received")
                return ApiResponse<bool>.ErrorResponse("Already fully received");

            var totalAfter = entry.ReceivedQty + request.ReceivedQty + entry.RejectedQty + request.RejectedQty;
            if (totalAfter > entry.Quantity)
                return ApiResponse<bool>.ErrorResponse(
                    $"Total received+rejected ({totalAfter}) would exceed sent quantity ({entry.Quantity})");

            await _ospRepo.MarkReceivedAsync(
                id,
                request.ReceivedQty,
                request.RejectedQty,
                request.ActualReturnDate.Date,
                request.Notes,
                request.UpdatedBy ?? "System");

            var isFullyReceived = totalAfter >= entry.Quantity;
            var message = isFullyReceived
                ? "Fully received — job card completed"
                : $"Partial receive recorded ({totalAfter}/{entry.Quantity})";

            return ApiResponse<bool>.SuccessResponse(true, message);
        }
    }
}
