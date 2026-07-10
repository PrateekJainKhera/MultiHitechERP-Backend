using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMaterialReconcileRepository
    {
        Task<IEnumerable<ReconcileLogResponse>> GetHistoryAsync(int? materialId);

        Task InsertLogAsync(int materialId, string? materialCode, string? materialName,
            int? pieceId, string? pieceNo, string actionType,
            decimal? lengthBefore, decimal? lengthAfter, decimal? lengthRemoved, decimal? weightRemoved,
            string? reason, string? remarks, string? performedBy);

        /// <summary>Set the raw material's aggregate stock (Inventory_Stock.CurrentStock, in mm) = total available length. Upserts.</summary>
        Task SetMaterialAggregateAsync(int materialId, decimal totalLengthMM, string uom);
    }
}
