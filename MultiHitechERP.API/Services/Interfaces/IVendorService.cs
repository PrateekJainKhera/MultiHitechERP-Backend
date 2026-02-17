using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IVendorService
    {
        Task<ApiResponse<IEnumerable<VendorResponse>>> GetAllVendorsAsync();
        Task<ApiResponse<IEnumerable<VendorResponse>>> GetActiveVendorsAsync();
        Task<ApiResponse<VendorResponse>> GetVendorByIdAsync(int id);
        Task<ApiResponse<IEnumerable<VendorResponse>>> SearchVendorsAsync(string searchTerm);
        Task<ApiResponse<int>> CreateVendorAsync(CreateVendorRequest request);
        Task<ApiResponse<bool>> UpdateVendorAsync(int id, UpdateVendorRequest request);
        Task<ApiResponse<bool>> DeleteVendorAsync(int id);
    }
}
