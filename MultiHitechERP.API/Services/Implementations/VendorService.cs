using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<ApiResponse<IEnumerable<VendorResponse>>> GetAllVendorsAsync()
        {
            var vendors = await _vendorRepository.GetAllAsync();
            return ApiResponse<IEnumerable<VendorResponse>>.SuccessResponse(
                vendors.Select(MapToResponse));
        }

        public async Task<ApiResponse<IEnumerable<VendorResponse>>> GetActiveVendorsAsync()
        {
            var vendors = await _vendorRepository.GetActiveAsync();
            return ApiResponse<IEnumerable<VendorResponse>>.SuccessResponse(
                vendors.Select(MapToResponse));
        }

        public async Task<ApiResponse<VendorResponse>> GetVendorByIdAsync(int id)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);
            if (vendor == null)
                return ApiResponse<VendorResponse>.ErrorResponse("Vendor not found");
            return ApiResponse<VendorResponse>.SuccessResponse(MapToResponse(vendor));
        }

        public async Task<ApiResponse<IEnumerable<VendorResponse>>> SearchVendorsAsync(string searchTerm)
        {
            var vendors = await _vendorRepository.SearchByNameAsync(searchTerm.Trim());
            return ApiResponse<IEnumerable<VendorResponse>>.SuccessResponse(
                vendors.Select(MapToResponse));
        }

        public async Task<ApiResponse<int>> CreateVendorAsync(CreateVendorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.VendorName))
                return ApiResponse<int>.ErrorResponse("Vendor name is required");

            var seq = await _vendorRepository.GetNextSequenceNumberAsync();
            var vendorCode = $"VEND-{seq:D4}";

            var vendor = new Vendor
            {
                VendorCode = vendorCode,
                VendorName = request.VendorName.Trim(),
                VendorType = request.VendorType.Trim(),
                ContactPerson = request.ContactPerson?.Trim(),
                Email = request.Email?.Trim().ToLower(),
                Phone = request.Phone?.Trim(),
                Address = request.Address?.Trim(),
                City = request.City?.Trim(),
                State = request.State?.Trim(),
                Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country.Trim(),
                PinCode = request.PinCode?.Trim(),
                GSTNo = request.GSTNo?.Trim().ToUpper(),
                PANNo = request.PANNo?.Trim().ToUpper(),
                CreditDays = request.CreditDays,
                CreditLimit = request.CreditLimit,
                PaymentTerms = string.IsNullOrWhiteSpace(request.PaymentTerms) ? "Net 30 Days" : request.PaymentTerms.Trim(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy ?? "Admin",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = request.CreatedBy ?? "Admin",
            };

            var id = await _vendorRepository.InsertAsync(vendor);
            return ApiResponse<int>.SuccessResponse(id, "Vendor created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateVendorAsync(int id, UpdateVendorRequest request)
        {
            var existing = await _vendorRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Vendor not found");

            existing.VendorName = request.VendorName.Trim();
            existing.VendorType = request.VendorType.Trim();
            existing.ContactPerson = request.ContactPerson?.Trim();
            existing.Email = request.Email?.Trim().ToLower();
            existing.Phone = request.Phone?.Trim();
            existing.Address = request.Address?.Trim();
            existing.City = request.City?.Trim();
            existing.State = request.State?.Trim();
            existing.Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country.Trim();
            existing.PinCode = request.PinCode?.Trim();
            existing.GSTNo = request.GSTNo?.Trim().ToUpper();
            existing.PANNo = request.PANNo?.Trim().ToUpper();
            existing.CreditDays = request.CreditDays;
            existing.CreditLimit = request.CreditLimit;
            existing.PaymentTerms = string.IsNullOrWhiteSpace(request.PaymentTerms) ? "Net 30 Days" : request.PaymentTerms.Trim();
            existing.IsActive = request.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = request.UpdatedBy ?? "Admin";

            await _vendorRepository.UpdateAsync(existing);
            return ApiResponse<bool>.SuccessResponse(true, "Vendor updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteVendorAsync(int id)
        {
            var existing = await _vendorRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Vendor not found");

            await _vendorRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Vendor deleted successfully");
        }

        private static VendorResponse MapToResponse(Vendor v) => new()
        {
            Id = v.Id,
            VendorCode = v.VendorCode,
            VendorName = v.VendorName,
            VendorType = v.VendorType,
            ContactPerson = v.ContactPerson,
            Email = v.Email,
            Phone = v.Phone,
            Address = v.Address,
            City = v.City,
            State = v.State,
            Country = v.Country,
            PinCode = v.PinCode,
            GSTNo = v.GSTNo,
            PANNo = v.PANNo,
            CreditDays = v.CreditDays,
            CreditLimit = v.CreditLimit,
            PaymentTerms = v.PaymentTerms,
            IsActive = v.IsActive,
            CreatedAt = v.CreatedAt,
            CreatedBy = v.CreatedBy,
            UpdatedAt = v.UpdatedAt,
            UpdatedBy = v.UpdatedBy,
        };
    }
}
