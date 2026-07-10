using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    [ApiController]
    [Route("api/material-reconcile")]
    public class MaterialReconcileController : ControllerBase
    {
        private readonly IMaterialReconcileService _service;

        public MaterialReconcileController(IMaterialReconcileService service)
        {
            _service = service;
        }

        // Available pieces of a material grouped by length + totals (for the reconcile screen)
        [HttpGet("pieces/{materialId:int}")]
        public async Task<IActionResult> GetPieces(int materialId)
        {
            var result = await _service.GetPiecesByLengthAsync(materialId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Apply piece-level reconciliation (remove bars / reduce lengths) + log + recompute aggregate
        [HttpPost]
        public async Task<IActionResult> Reconcile([FromBody] ReconcilePiecesRequest request)
        {
            var result = await _service.ReconcileAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Reconcile history (all, or filtered by material via ?materialId=)
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int? materialId)
        {
            var result = await _service.GetHistoryAsync(materialId);
            return Ok(result);
        }
    }
}
