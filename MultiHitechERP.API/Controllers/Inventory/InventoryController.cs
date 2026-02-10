using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Inventory
{
    /// <summary>
    /// Inventory API endpoints for stock management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all inventory records
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<InventoryResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<InventoryResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get inventory by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<InventoryResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get inventory by material ID
        /// </summary>
        [HttpGet("by-material/{materialId:int}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> GetByMaterialId(int materialId)
        {
            var response = await _service.GetByMaterialIdAsync(materialId);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<InventoryResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get low stock items
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<ApiResponse<InventoryResponse[]>>> GetLowStock()
        {
            var response = await _service.GetLowStockAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<InventoryResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get out of stock items
        /// </summary>
        [HttpGet("out-of-stock")]
        public async Task<ActionResult<ApiResponse<InventoryResponse[]>>> GetOutOfStock()
        {
            var response = await _service.GetOutOfStockAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<InventoryResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get inventory by category
        /// </summary>
        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse[]>>> GetByCategory(string category)
        {
            var response = await _service.GetByCategoryAsync(category);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<InventoryResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get inventory by location
        /// </summary>
        [HttpGet("by-location/{location}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse[]>>> GetByLocation(string location)
        {
            var response = await _service.GetByLocationAsync(location);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<InventoryResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get active inventory items
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<InventoryResponse[]>>> GetActive()
        {
            var response = await _service.GetActiveAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<InventoryResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Record stock in (GRN)
        /// </summary>
        [HttpPost("stock-in")]
        public async Task<ActionResult<ApiResponse<Guid>>> StockIn([FromBody] StockInRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.RecordStockInAsync(
                request.MaterialId,
                request.Quantity,
                request.GRNNo,
                request.SupplierId,
                request.UnitCost,
                request.PerformedBy,
                request.Remarks ?? string.Empty
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Record stock out (Material Issue)
        /// </summary>
        [HttpPost("stock-out")]
        public async Task<ActionResult<ApiResponse<Guid>>> StockOut([FromBody] StockOutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.RecordStockOutAsync(
                request.MaterialId,
                request.Quantity,
                request.JobCardId,
                request.RequisitionId,
                request.PerformedBy,
                request.Remarks ?? string.Empty
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Record stock adjustment
        /// </summary>
        [HttpPost("adjustment")]
        public async Task<ActionResult<ApiResponse<Guid>>> StockAdjustment([FromBody] StockAdjustmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.RecordStockAdjustmentAsync(
                request.MaterialId,
                request.Quantity,
                request.Remarks,
                request.PerformedBy
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Reconcile stock (physical count)
        /// </summary>
        [HttpPost("reconcile")]
        public async Task<ActionResult<ApiResponse<bool>>> ReconcileStock([FromBody] StockReconciliationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.ReconcileStockAsync(
                request.MaterialId,
                request.ActualQuantity,
                request.PerformedBy,
                request.Remarks
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Receive purchased component directly into inventory
        /// </summary>
        [HttpPost("receive-component")]
        public async Task<ActionResult<ApiResponse<int>>> ReceiveComponent([FromBody] ReceiveComponentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<int>.ErrorResponse("Invalid request data"));

            var response = await _service.ReceiveComponentAsync(
                request.ComponentId,
                request.ComponentName,
                request.PartNumber ?? "",
                request.Quantity,
                request.Unit,
                request.UnitCost,
                request.SupplierId,
                request.SupplierName ?? "",
                request.InvoiceNo ?? "",
                request.InvoiceDate,
                request.PONo ?? "",
                request.PODate,
                request.ReceiptDate,
                request.StorageLocation ?? "Main Warehouse",
                request.Remarks ?? "",
                request.ReceivedBy
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get available stock for a purchased component
        /// </summary>
        [HttpGet("component-stock/{componentId:int}")]
        public async Task<ActionResult<ApiResponse<object>>> GetComponentStock(int componentId)
        {
            var (currentStock, availableStock, uom, location) = await _service.GetComponentStockAsync(componentId);
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                componentId,
                currentStock,
                availableStock,
                uom,
                location
            }));
        }

        /// <summary>
        /// Update stock level parameters
        /// </summary>
        [HttpPut("stock-levels")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStockLevels([FromBody] UpdateStockLevelsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateStockLevelsAsync(
                request.MaterialId,
                request.MinimumStock,
                request.MaximumStock,
                request.ReorderLevel,
                request.ReorderQuantity
            );

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get transactions by material ID
        /// </summary>
        [HttpGet("transactions/by-material/{materialId:int}")]
        public async Task<ActionResult<ApiResponse<InventoryTransactionResponse[]>>> GetTransactionsByMaterialId(int materialId)
        {
            var response = await _service.GetTransactionsByMaterialIdAsync(materialId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToTransactionResponse).ToArray();
            return Ok(ApiResponse<InventoryTransactionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get transactions by type
        /// </summary>
        [HttpGet("transactions/by-type/{transactionType}")]
        public async Task<ActionResult<ApiResponse<InventoryTransactionResponse[]>>> GetTransactionsByType(string transactionType)
        {
            var response = await _service.GetTransactionsByTypeAsync(transactionType);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToTransactionResponse).ToArray();
            return Ok(ApiResponse<InventoryTransactionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get transactions by date range
        /// </summary>
        [HttpGet("transactions/by-date-range")]
        public async Task<ActionResult<ApiResponse<InventoryTransactionResponse[]>>> GetTransactionsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var response = await _service.GetTransactionsByDateRangeAsync(startDate, endDate);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToTransactionResponse).ToArray();
            return Ok(ApiResponse<InventoryTransactionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get recent transactions
        /// </summary>
        [HttpGet("transactions/recent")]
        public async Task<ActionResult<ApiResponse<InventoryTransactionResponse[]>>> GetRecentTransactions([FromQuery] int count = 100)
        {
            var response = await _service.GetRecentTransactionsAsync(count);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToTransactionResponse).ToArray();
            return Ok(ApiResponse<InventoryTransactionResponse[]>.SuccessResponse(dtos));
        }

        // Helper Methods
        private static InventoryResponse MapToResponse(MultiHitechERP.API.Models.Inventory.Inventory inventory)
        {
            return new InventoryResponse
            {
                Id = inventory.Id,
                MaterialId = inventory.MaterialId,
                MaterialCode = inventory.MaterialCode,
                MaterialName = inventory.MaterialName,
                MaterialCategory = inventory.MaterialCategory,
                TotalQuantity = inventory.TotalQuantity,
                AvailableQuantity = inventory.AvailableQuantity,
                AllocatedQuantity = inventory.AllocatedQuantity,
                IssuedQuantity = inventory.IssuedQuantity,
                ReservedQuantity = inventory.ReservedQuantity,
                UOM = inventory.UOM,
                MinimumStock = inventory.MinimumStock,
                MaximumStock = inventory.MaximumStock,
                ReorderLevel = inventory.ReorderLevel,
                ReorderQuantity = inventory.ReorderQuantity,
                PrimaryStorageLocation = inventory.PrimaryStorageLocation,
                WarehouseCode = inventory.WarehouseCode,
                AverageCostPerUnit = inventory.AverageCostPerUnit,
                TotalStockValue = inventory.TotalStockValue,
                IsLowStock = inventory.IsLowStock,
                IsOutOfStock = inventory.IsOutOfStock,
                IsActive = inventory.IsActive,
                LastStockInDate = inventory.LastStockInDate,
                LastStockOutDate = inventory.LastStockOutDate,
                LastCountDate = inventory.LastCountDate,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt,
                UpdatedBy = inventory.UpdatedBy
            };
        }

        private static InventoryTransactionResponse MapToTransactionResponse(MultiHitechERP.API.Models.Inventory.InventoryTransaction transaction)
        {
            return new InventoryTransactionResponse
            {
                Id = transaction.Id,
                MaterialId = transaction.MaterialId,
                TransactionType = transaction.TransactionType,
                TransactionNo = transaction.TransactionNo,
                TransactionDate = transaction.TransactionDate,
                Quantity = transaction.Quantity,
                UOM = transaction.UOM,
                ReferenceType = transaction.ReferenceType,
                ReferenceId = transaction.ReferenceId,
                ReferenceNo = transaction.ReferenceNo,
                FromLocation = transaction.FromLocation,
                ToLocation = transaction.ToLocation,
                UnitCost = transaction.UnitCost,
                TotalCost = transaction.TotalCost,
                BalanceQuantity = transaction.BalanceQuantity,
                Remarks = transaction.Remarks,
                PerformedBy = transaction.PerformedBy,
                JobCardId = transaction.JobCardId,
                RequisitionId = transaction.RequisitionId,
                SupplierId = transaction.SupplierId,
                GRNNo = transaction.GRNNo,
                CreatedAt = transaction.CreatedAt,
                CreatedBy = transaction.CreatedBy
            };
        }
    }
}
