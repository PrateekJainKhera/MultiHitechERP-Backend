using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IMaterialReconcileService
    {
        Task<ApiResponse<MaterialPiecesByLengthResponse>> GetPiecesByLengthAsync(int materialId);
        Task<ApiResponse<ReconcileResultResponse>> ReconcileAsync(ReconcilePiecesRequest request);
        Task<ApiResponse<IEnumerable<ReconcileLogResponse>>> GetHistoryAsync(int? materialId);
    }
}
