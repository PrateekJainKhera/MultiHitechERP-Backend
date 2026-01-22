using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for BOM (Bill of Materials) business logic
    /// </summary>
    public class BOMService : IBOMService
    {
        private readonly IBOMRepository _bomRepository;
        private readonly IProductRepository _productRepository;

        public BOMService(
            IBOMRepository bomRepository,
            IProductRepository productRepository)
        {
            _bomRepository = bomRepository;
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<BOM>> GetByIdAsync(int id)
        {
            var bom = await _bomRepository.GetByIdAsync(id);
            if (bom == null)
                return ApiResponse<BOM>.ErrorResponse("BOM not found");

            return ApiResponse<BOM>.SuccessResponse(bom);
        }

        public async Task<ApiResponse<BOM>> GetByBOMNoAsync(string bomNo)
        {
            if (string.IsNullOrWhiteSpace(bomNo))
                return ApiResponse<BOM>.ErrorResponse("BOM number is required");

            var bom = await _bomRepository.GetByBOMNoAsync(bomNo);
            if (bom == null)
                return ApiResponse<BOM>.ErrorResponse("BOM not found");

            return ApiResponse<BOM>.SuccessResponse(bom);
        }

        public async Task<ApiResponse<IEnumerable<BOM>>> GetAllAsync()
        {
            var boms = await _bomRepository.GetAllAsync();
            return ApiResponse<IEnumerable<BOM>>.SuccessResponse(boms);
        }

        public async Task<ApiResponse<int>> CreateBOMAsync(BOM bom)
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(bom.ProductId);
            if (product == null)
                return ApiResponse<int>.ErrorResponse("Product not found");

            // Generate BOM number if not provided
            if (string.IsNullOrWhiteSpace(bom.BOMNo))
            {
                var revisionNumber = await _bomRepository.GetNextRevisionNumberAsync(bom.ProductId);
                bom.BOMNo = $"BOM-{product.PartCode}-R{revisionNumber:D2}";
                bom.RevisionNumber = revisionNumber.ToString();
            }

            // Set product details
            bom.ProductCode = product.PartCode;
            bom.ProductName = product.ModelName;

            // Mark all previous revisions as non-latest
            var existingBOMs = await _bomRepository.GetByProductIdAsync(bom.ProductId);
            foreach (var existingBOM in existingBOMs)
            {
                if (existingBOM.IsLatestRevision)
                {
                    await _bomRepository.MarkAsNonLatestAsync(existingBOM.Id);
                }
            }

            // Set as latest revision
            bom.IsLatestRevision = true;
            bom.RevisionDate = DateTime.UtcNow;

            var id = await _bomRepository.InsertAsync(bom);
            return ApiResponse<int>.SuccessResponse(id, $"BOM {bom.BOMNo} created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateBOMAsync(BOM bom)
        {
            var existing = await _bomRepository.GetByIdAsync(bom.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("BOM not found");

            var success = await _bomRepository.UpdateAsync(bom);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update BOM");

            return ApiResponse<bool>.SuccessResponse(true, "BOM updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteBOMAsync(int id)
        {
            var existing = await _bomRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("BOM not found");

            // Delete all BOM items first
            await _bomRepository.DeleteAllBOMItemsAsync(id);

            var success = await _bomRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete BOM");

            return ApiResponse<bool>.SuccessResponse(true, "BOM deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<BOM>>> GetByProductIdAsync(int productId)
        {
            var boms = await _bomRepository.GetByProductIdAsync(productId);
            return ApiResponse<IEnumerable<BOM>>.SuccessResponse(boms);
        }

        public async Task<ApiResponse<BOM>> GetLatestRevisionAsync(int productId)
        {
            var bom = await _bomRepository.GetLatestRevisionAsync(productId);
            if (bom == null)
                return ApiResponse<BOM>.ErrorResponse("No BOM found for this product");

            return ApiResponse<BOM>.SuccessResponse(bom);
        }

        public async Task<ApiResponse<IEnumerable<BOM>>> GetByProductCodeAsync(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                return ApiResponse<IEnumerable<BOM>>.ErrorResponse("Product code is required");

            var boms = await _bomRepository.GetByProductCodeAsync(productCode);
            return ApiResponse<IEnumerable<BOM>>.SuccessResponse(boms);
        }

        public async Task<ApiResponse<IEnumerable<BOM>>> GetActiveAsync()
        {
            var boms = await _bomRepository.GetActiveAsync();
            return ApiResponse<IEnumerable<BOM>>.SuccessResponse(boms);
        }

        public async Task<ApiResponse<IEnumerable<BOM>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<BOM>>.ErrorResponse("Status is required");

            var boms = await _bomRepository.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<BOM>>.SuccessResponse(boms);
        }

        public async Task<ApiResponse<IEnumerable<BOM>>> GetByBOMTypeAsync(string bomType)
        {
            if (string.IsNullOrWhiteSpace(bomType))
                return ApiResponse<IEnumerable<BOM>>.ErrorResponse("BOM type is required");

            var boms = await _bomRepository.GetByBOMTypeAsync(bomType);
            return ApiResponse<IEnumerable<BOM>>.SuccessResponse(boms);
        }

        // BOM Item Operations
        public async Task<ApiResponse<IEnumerable<BOMItem>>> GetBOMItemsAsync(int bomId)
        {
            var items = await _bomRepository.GetBOMItemsAsync(bomId);
            return ApiResponse<IEnumerable<BOMItem>>.SuccessResponse(items);
        }

        public async Task<ApiResponse<int>> AddBOMItemAsync(BOMItem item)
        {
            // Validate BOM exists
            var bom = await _bomRepository.GetByIdAsync(item.BOMId);
            if (bom == null)
                return ApiResponse<int>.ErrorResponse("BOM not found");

            // Validate item type
            if (string.IsNullOrWhiteSpace(item.ItemType))
                return ApiResponse<int>.ErrorResponse("Item type is required");

            // Validate material or child part is provided
            if (item.ItemType == "Material" && !item.MaterialId.HasValue)
                return ApiResponse<int>.ErrorResponse("Material ID is required for Material items");

            if (item.ItemType == "Child Part" && !item.ChildPartId.HasValue)
                return ApiResponse<int>.ErrorResponse("Child Part ID is required for Child Part items");

            // Validate quantity
            if (item.QuantityRequired <= 0)
                return ApiResponse<int>.ErrorResponse("Quantity required must be greater than zero");

            // Calculate net quantity if scrap/wastage is provided
            if (item.ScrapPercentage.HasValue || item.WastagePercentage.HasValue)
            {
                var scrap = item.ScrapPercentage ?? 0;
                var wastage = item.WastagePercentage ?? 0;
                item.NetQuantityRequired = item.QuantityRequired * (1 + (scrap + wastage) / 100);
            }
            else
            {
                item.NetQuantityRequired = item.QuantityRequired;
            }

            var id = await _bomRepository.InsertBOMItemAsync(item);
            return ApiResponse<int>.SuccessResponse(id, "BOM item added successfully");
        }

        public async Task<ApiResponse<bool>> UpdateBOMItemAsync(BOMItem item)
        {
            // Validate quantity
            if (item.QuantityRequired <= 0)
                return ApiResponse<bool>.ErrorResponse("Quantity required must be greater than zero");

            // Recalculate net quantity
            if (item.ScrapPercentage.HasValue || item.WastagePercentage.HasValue)
            {
                var scrap = item.ScrapPercentage ?? 0;
                var wastage = item.WastagePercentage ?? 0;
                item.NetQuantityRequired = item.QuantityRequired * (1 + (scrap + wastage) / 100);
            }
            else
            {
                item.NetQuantityRequired = item.QuantityRequired;
            }

            var success = await _bomRepository.UpdateBOMItemAsync(item);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update BOM item");

            return ApiResponse<bool>.SuccessResponse(true, "BOM item updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteBOMItemAsync(int itemId)
        {
            var success = await _bomRepository.DeleteBOMItemAsync(itemId);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete BOM item");

            return ApiResponse<bool>.SuccessResponse(true, "BOM item deleted successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAllBOMItemsAsync(int bomId)
        {
            var success = await _bomRepository.DeleteAllBOMItemsAsync(bomId);
            return ApiResponse<bool>.SuccessResponse(success, "All BOM items deleted");
        }

        public async Task<ApiResponse<IEnumerable<BOMItem>>> GetMaterialItemsAsync(int bomId)
        {
            var items = await _bomRepository.GetMaterialItemsAsync(bomId);
            return ApiResponse<IEnumerable<BOMItem>>.SuccessResponse(items);
        }

        public async Task<ApiResponse<IEnumerable<BOMItem>>> GetChildPartItemsAsync(int bomId)
        {
            var items = await _bomRepository.GetChildPartItemsAsync(bomId);
            return ApiResponse<IEnumerable<BOMItem>>.SuccessResponse(items);
        }

        public async Task<ApiResponse<IEnumerable<BOMItem>>> GetItemsByTypeAsync(int bomId, string itemType)
        {
            if (string.IsNullOrWhiteSpace(itemType))
                return ApiResponse<IEnumerable<BOMItem>>.ErrorResponse("Item type is required");

            var items = await _bomRepository.GetItemsByTypeAsync(bomId, itemType);
            return ApiResponse<IEnumerable<BOMItem>>.SuccessResponse(items);
        }

        public async Task<ApiResponse<bool>> ApproveBOMAsync(int id, string approvedBy)
        {
            var bom = await _bomRepository.GetByIdAsync(id);
            if (bom == null)
                return ApiResponse<bool>.ErrorResponse("BOM not found");

            if (string.IsNullOrWhiteSpace(approvedBy))
                return ApiResponse<bool>.ErrorResponse("Approver name is required");

            // Check if BOM has items
            var items = await _bomRepository.GetBOMItemsAsync(id);
            if (!items.Any())
                return ApiResponse<bool>.ErrorResponse("Cannot approve BOM with no items");

            var success = await _bomRepository.ApproveBOMAsync(id, approvedBy, DateTime.UtcNow);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to approve BOM");

            return ApiResponse<bool>.SuccessResponse(true, "BOM approved successfully");
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status)
        {
            var bom = await _bomRepository.GetByIdAsync(id);
            if (bom == null)
                return ApiResponse<bool>.ErrorResponse("BOM not found");

            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<bool>.ErrorResponse("Status is required");

            var success = await _bomRepository.UpdateStatusAsync(id, status);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update BOM status");

            return ApiResponse<bool>.SuccessResponse(true, "BOM status updated successfully");
        }

        public async Task<ApiResponse<int>> CreateRevisionAsync(int bomId, string revisionNumber, string createdBy)
        {
            var existing = await _bomRepository.GetByIdAsync(bomId);
            if (existing == null)
                return ApiResponse<int>.ErrorResponse("BOM not found");

            // Mark old revision as non-latest
            await _bomRepository.MarkAsNonLatestAsync(bomId);

            // Create new BOM with incremented revision
            var newBOM = new BOM
            {
                BOMNo = $"BOM-{existing.ProductCode}-R{revisionNumber}",
                ProductId = existing.ProductId,
                ProductCode = existing.ProductCode,
                ProductName = existing.ProductName,
                RevisionNumber = revisionNumber,
                RevisionDate = DateTime.UtcNow,
                IsLatestRevision = true,
                BOMType = existing.BOMType,
                BaseQuantity = existing.BaseQuantity,
                BaseUOM = existing.BaseUOM,
                IsActive = true,
                Status = "Draft",
                Remarks = $"Revised from {existing.BOMNo}",
                CreatedBy = createdBy
            };

            var newBOMId = await _bomRepository.InsertAsync(newBOM);

            // Copy all items from old BOM
            var items = await _bomRepository.GetBOMItemsAsync(bomId);
            foreach (var item in items)
            {
                var newItem = new BOMItem
                {
                    BOMId = newBOMId,
                    LineNo = item.LineNo,
                    ItemType = item.ItemType,
                    MaterialId = item.MaterialId,
                    MaterialCode = item.MaterialCode,
                    MaterialName = item.MaterialName,
                    ChildPartId = item.ChildPartId,
                    ChildPartCode = item.ChildPartCode,
                    ChildPartName = item.ChildPartName,
                    QuantityRequired = item.QuantityRequired,
                    UOM = item.UOM,
                    LengthRequiredMM = item.LengthRequiredMM,
                    ScrapPercentage = item.ScrapPercentage,
                    ScrapQuantity = item.ScrapQuantity,
                    WastagePercentage = item.WastagePercentage,
                    NetQuantityRequired = item.NetQuantityRequired,
                    ReferenceDesignator = item.ReferenceDesignator,
                    Notes = item.Notes
                };

                await _bomRepository.InsertBOMItemAsync(newItem);
            }

            return ApiResponse<int>.SuccessResponse(newBOMId, $"Revision {revisionNumber} created successfully");
        }
    }
}
