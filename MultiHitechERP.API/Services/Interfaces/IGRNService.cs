using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IGRNService
    {
        Task<GRNResponse> CreateGRNAsync(CreateGRNRequest request);
        Task<GRNResponse?> GetByIdAsync(int id);
        Task<GRNResponse?> GetByGRNNoAsync(string grnNo);
        Task<IEnumerable<GRNResponse>> GetAllAsync();
        Task<IEnumerable<GRNResponse>> GetBySupplierId(int supplierId);
        Task<bool> UpdateStatusAsync(int id, string status, string updatedBy);
        Task<bool> DeleteAsync(int id);
        Task<GRNResponse?> GetGRNWithLinesAsync(int id);
    }
}
