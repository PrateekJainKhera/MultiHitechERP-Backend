using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class OrderItemQCService : IOrderItemQCService
    {
        private readonly IOrderItemQCRepository _repo;
        private readonly IS3Service _s3;

        public OrderItemQCService(IOrderItemQCRepository repo, IS3Service s3)
        {
            _repo = repo;
            _s3 = s3;
        }

        public async Task<ApiResponse<IEnumerable<QCPendingItemResponse>>> GetPendingItemsAsync()
        {
            try
            {
                var items = await _repo.GetPendingItemsAsync();
                return ApiResponse<IEnumerable<QCPendingItemResponse>>.SuccessResponse(items);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QCPendingItemResponse>>.ErrorResponse(ex.Message);
            }
        }

        public async Task<OrderItemQCResponse?> GetLatestByOrderItemAsync(int orderItemId)
        {
            var record = await _repo.GetLatestByOrderItemAsync(orderItemId);
            if (record == null) return null;
            return new OrderItemQCResponse
            {
                Id              = record.Id,
                OrderItemId     = record.OrderItemId,
                OrderId         = record.OrderId,
                QCStatus        = record.QCStatus,
                CertificatePath = record.CertificatePath,
                QCCompletedAt   = record.QCCompletedAt,
                QCCompletedBy   = record.QCCompletedBy,
                Notes           = record.Notes,
                CreatedAt       = record.CreatedAt,
            };
        }

        public async Task<ApiResponse<OrderItemQCResponse>> SubmitQCAsync(
            int orderItemId,
            int orderId,
            string qcStatus,
            string qcBy,
            string? notes,
            Stream? certificateStream,
            string? certificateContentType,
            string? originalFileName)
        {
            try
            {
                if (qcStatus != "Passed" && qcStatus != "Failed")
                    return ApiResponse<OrderItemQCResponse>.ErrorResponse("QCStatus must be 'Passed' or 'Failed'.");

                if (string.IsNullOrWhiteSpace(qcBy))
                    return ApiResponse<OrderItemQCResponse>.ErrorResponse("QC completed-by name is required.");

                // Upload certificate PDF if provided (non-fatal — QC proceeds even if upload fails)
                string? certPath = null;
                if (certificateStream != null && certificateStream.Length > 0 && !string.IsNullOrEmpty(certificateContentType))
                {
                    try
                    {
                        var safeFileName = string.IsNullOrEmpty(originalFileName)
                            ? $"{Guid.NewGuid()}.pdf"
                            : $"{Guid.NewGuid()}_{Path.GetFileName(originalFileName)}";
                        var s3Key = $"multihitech/qc-certificates/{safeFileName}";
                        certPath = await _s3.UploadAsync(certificateStream, s3Key, certificateContentType);
                    }
                    catch
                    {
                        // S3 not configured or upload failed — continue without certificate
                    }
                }

                var record = new OrderItemQC
                {
                    OrderItemId     = orderItemId,
                    OrderId         = orderId,
                    QCStatus        = qcStatus,
                    CertificatePath = certPath,
                    QCCompletedAt   = DateTime.UtcNow,
                    QCCompletedBy   = qcBy,
                    Notes           = notes,
                };

                var newId = await _repo.InsertAsync(record);

                var response = new OrderItemQCResponse
                {
                    Id              = newId,
                    OrderItemId     = record.OrderItemId,
                    OrderId         = record.OrderId,
                    QCStatus        = record.QCStatus,
                    CertificatePath = record.CertificatePath,
                    QCCompletedAt   = record.QCCompletedAt,
                    QCCompletedBy   = record.QCCompletedBy,
                    Notes           = record.Notes,
                    CreatedAt       = DateTime.UtcNow,
                };

                return ApiResponse<OrderItemQCResponse>.SuccessResponse(response,
                    qcStatus == "Passed" ? "QC passed — item is ready for dispatch." : "QC marked as failed.");
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderItemQCResponse>.ErrorResponse(ex.Message);
            }
        }
    }
}
