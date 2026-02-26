using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ProductionService : IProductionService
    {
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IMaterialPieceRepository _pieceRepository;
        private readonly IOSPTrackingRepository _ospRepository;

        public ProductionService(
            IJobCardRepository jobCardRepository,
            IScheduleRepository scheduleRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProcessRepository processRepository,
            IMaterialPieceRepository pieceRepository,
            IOSPTrackingRepository ospRepository)
        {
            _jobCardRepository = jobCardRepository;
            _scheduleRepository = scheduleRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _processRepository = processRepository;
            _pieceRepository = pieceRepository;
            _ospRepository = ospRepository;
        }

        // Helper: stable key for grouping job cards by child part
        // Uses ChildPartId when available, falls back to ChildPartName
        private static string ChildPartKey(Models.Planning.JobCard jc) =>
            jc.ChildPartId.HasValue ? $"id:{jc.ChildPartId}" : $"name:{jc.ChildPartName ?? "unknown"}";

        // ─────────────────────────────────────────────────────────────────────
        // 1. Production Dashboard — list of orders
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<IEnumerable<ProductionOrderSummary>>> GetOrdersAsync()
        {
            try
            {
                // Get all orders
                var orders = await _orderRepository.GetAllAsync();
                var result = new List<ProductionOrderSummary>();

                foreach (var order in orders)
                {
                    // Only include orders that have at least one scheduled job card
                    var jobCards = (await _jobCardRepository.GetByOrderIdAsync(order.Id)).ToList();
                    var scheduledJcs = jobCards.Where(jc => jc.Status == "Scheduled").ToList();
                    if (!scheduledJcs.Any()) continue;

                    var nonAssemblyJcs = jobCards.Where(jc => jc.CreationType != "Assembly").ToList();
                    var completedSteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Completed");
                    var inProgressSteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "InProgress");
                    var completedChildParts = nonAssemblyJcs
                        .Where(jc => jc.ReadyForAssembly)
                        .Select(jc => ChildPartKey(jc))
                        .Distinct()
                        .Count();
                    var totalChildParts = nonAssemblyJcs
                        .Select(jc => ChildPartKey(jc))
                        .Distinct()
                        .Count();

                    string prodStatus;
                    if (nonAssemblyJcs.All(jc => jc.ProductionStatus == "Completed"))
                        prodStatus = "Completed";
                    else if (inProgressSteps > 0 || completedSteps > 0)
                        prodStatus = "InProgress";
                    else
                        prodStatus = "Pending";

                    result.Add(new ProductionOrderSummary
                    {
                        OrderId = order.Id,
                        OrderNo = order.OrderNo,
                        CustomerName = order.CustomerName,
                        ProductName = order.ProductName,
                        Priority = order.Priority ?? "Medium",
                        DueDate = order.DueDate,
                        TotalSteps = nonAssemblyJcs.Count,
                        CompletedSteps = completedSteps,
                        InProgressSteps = inProgressSteps,
                        ReadySteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Ready"),
                        TotalChildParts = totalChildParts,
                        CompletedChildParts = completedChildParts,
                        ProductionStatus = prodStatus
                    });
                }

                return ApiResponse<IEnumerable<ProductionOrderSummary>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductionOrderSummary>>.ErrorResponse($"Error loading production orders: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 1.5. Production Dashboard — list of ORDER ITEMS (NEW - for multi-product orders)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<IEnumerable<ProductionOrderSummary>>> GetOrderItemsAsync()
        {
            try
            {
                var result = new List<ProductionOrderSummary>();

                // ── Part 1: Multi-product orders — job cards linked to a specific OrderItemId ──
                var orderItems = await _orderItemRepository.GetAllAsync();

                foreach (var orderItem in orderItems)
                {
                    var jobCards = (await _jobCardRepository.GetByOrderItemIdAsync(orderItem.Id)).ToList();
                    var scheduledJcs = jobCards.Where(jc => jc.Status == "Scheduled").ToList();
                    if (!scheduledJcs.Any()) continue;

                    var nonAssemblyJcs = jobCards.Where(jc => jc.CreationType != "Assembly").ToList();
                    var completedSteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Completed");
                    var inProgressSteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "InProgress");
                    var completedChildParts = nonAssemblyJcs
                        .Where(jc => jc.ReadyForAssembly)
                        .Select(jc => ChildPartKey(jc)).Distinct().Count();
                    var totalChildParts = nonAssemblyJcs
                        .Select(jc => ChildPartKey(jc)).Distinct().Count();

                    string prodStatus;
                    if (nonAssemblyJcs.All(jc => jc.ProductionStatus == "Completed"))
                        prodStatus = "Completed";
                    else if (inProgressSteps > 0 || completedSteps > 0)
                        prodStatus = "InProgress";
                    else
                        prodStatus = "Pending";

                    var order = await _orderRepository.GetByIdAsync(orderItem.OrderId);
                    var baseOrderNo = order?.OrderNo ?? orderItem.OrderId.ToString();
                    var fullOrderRef = !string.IsNullOrEmpty(orderItem.ItemSequence)
                        ? $"{baseOrderNo}-{orderItem.ItemSequence}"
                        : baseOrderNo;

                    result.Add(new ProductionOrderSummary
                    {
                        OrderId = orderItem.OrderId,
                        OrderItemId = orderItem.Id,
                        OrderNo = fullOrderRef,
                        CustomerName = order?.CustomerName,
                        ProductName = orderItem.ProductName,
                        Priority = orderItem.Priority ?? "Medium",
                        DueDate = orderItem.DueDate,
                        TotalSteps = nonAssemblyJcs.Count,
                        CompletedSteps = completedSteps,
                        InProgressSteps = inProgressSteps,
                        ReadySteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Ready"),
                        TotalChildParts = totalChildParts,
                        CompletedChildParts = completedChildParts,
                        ProductionStatus = prodStatus
                    });
                }

                // ── Part 2: Job cards with no OrderItemId — group by ItemSequence ──
                var allOrders = await _orderRepository.GetAllAsync();
                foreach (var order in allOrders)
                {
                    // Only consider job cards not linked to any OrderItem
                    var allUnlinkedJcs = (await _jobCardRepository.GetByOrderIdAsync(order.Id))
                        .Where(jc => !jc.OrderItemId.HasValue)
                        .ToList();
                    if (!allUnlinkedJcs.Any(jc => jc.Status == "Scheduled")) continue;

                    // Load OrderItems for this order (to get product names and sequences)
                    var orderItemsForOrder = await _orderItemRepository.GetByOrderIdAsync(order.Id);

                    // Group by ItemSequence: multi-product orders have A/B/C..., single-product have null
                    var groups = allUnlinkedJcs
                        .GroupBy(jc => jc.ItemSequence ?? "")
                        .OrderBy(g => g.Key)
                        .ToList();

                    // If job cards have no ItemSequence but the order has OrderItems,
                    // use OrderItems to produce separate rows per product
                    bool jobCardsLackSequence = groups.Count == 1 && groups[0].Key == "" && orderItemsForOrder.Any();
                    if (jobCardsLackSequence)
                    {
                        // Fall back: show one row per OrderItem, sharing all the unlinked job cards
                        var allJcs = groups[0].ToList();
                        var scheduledAny = allJcs.Any(jc => jc.Status == "Scheduled");
                        if (!scheduledAny) continue;

                        foreach (var oi in orderItemsForOrder.OrderBy(oi => oi.ItemSequence))
                        {
                            var nonAssemblyJcs = allJcs.Where(jc => jc.CreationType != "Assembly").ToList();
                            var completedSteps2 = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Completed");
                            var inProgressSteps2 = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "InProgress");
                            string prodStatus2;
                            if (nonAssemblyJcs.All(jc => jc.ProductionStatus == "Completed"))
                                prodStatus2 = "Completed";
                            else if (inProgressSteps2 > 0 || completedSteps2 > 0)
                                prodStatus2 = "InProgress";
                            else
                                prodStatus2 = "Pending";

                            var orderRef2 = !string.IsNullOrEmpty(oi.ItemSequence)
                                ? $"{order.OrderNo}-{oi.ItemSequence}"
                                : order.OrderNo;

                            result.Add(new ProductionOrderSummary
                            {
                                OrderId = order.Id,
                                OrderItemId = oi.Id,
                                OrderNo = orderRef2,
                                CustomerName = order.CustomerName,
                                ProductName = oi.ProductName ?? order.ProductName,
                                Priority = oi.Priority ?? order.Priority ?? "Medium",
                                DueDate = oi.DueDate,
                                TotalSteps = nonAssemblyJcs.Count,
                                CompletedSteps = completedSteps2,
                                InProgressSteps = inProgressSteps2,
                                ReadySteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Ready"),
                                TotalChildParts = nonAssemblyJcs.Select(jc => ChildPartKey(jc)).Distinct().Count(),
                                CompletedChildParts = nonAssemblyJcs.Where(jc => jc.ReadyForAssembly).Select(jc => ChildPartKey(jc)).Distinct().Count(),
                                ProductionStatus = prodStatus2
                            });
                        }
                        continue;
                    }

                    foreach (var group in groups)
                    {
                        var jobCards = group.ToList();
                        var scheduledJcs = jobCards.Where(jc => jc.Status == "Scheduled").ToList();
                        if (!scheduledJcs.Any()) continue;

                        var nonAssemblyJcs = jobCards.Where(jc => jc.CreationType != "Assembly").ToList();
                        var completedSteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Completed");
                        var inProgressSteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "InProgress");
                        var completedChildParts = nonAssemblyJcs
                            .Where(jc => jc.ReadyForAssembly)
                            .Select(jc => ChildPartKey(jc)).Distinct().Count();
                        var totalChildParts = nonAssemblyJcs
                            .Select(jc => ChildPartKey(jc)).Distinct().Count();

                        string prodStatus;
                        if (nonAssemblyJcs.All(jc => jc.ProductionStatus == "Completed"))
                            prodStatus = "Completed";
                        else if (inProgressSteps > 0 || completedSteps > 0)
                            prodStatus = "InProgress";
                        else
                            prodStatus = "Pending";

                        // Build order reference: "ORD-XXXX-A" when sequence exists, "ORD-XXXX" otherwise
                        var seq = group.Key;
                        var orderRef = !string.IsNullOrEmpty(seq)
                            ? $"{order.OrderNo}-{seq}"
                            : order.OrderNo;

                        // Get ProductName from the OrderItem matching this sequence (if any)
                        var matchingOrderItem = orderItemsForOrder
                            .FirstOrDefault(oi => oi.ItemSequence == seq);

                        result.Add(new ProductionOrderSummary
                        {
                            OrderId = order.Id,
                            OrderItemId = matchingOrderItem?.Id,
                            OrderNo = orderRef,
                            CustomerName = order.CustomerName,
                            ProductName = matchingOrderItem?.ProductName ?? order.ProductName,
                            Priority = matchingOrderItem?.Priority ?? order.Priority ?? "Medium",
                            DueDate = matchingOrderItem?.DueDate ?? order.DueDate,
                            TotalSteps = nonAssemblyJcs.Count,
                            CompletedSteps = completedSteps,
                            InProgressSteps = inProgressSteps,
                            ReadySteps = nonAssemblyJcs.Count(jc => jc.ProductionStatus == "Ready"),
                            TotalChildParts = totalChildParts,
                            CompletedChildParts = completedChildParts,
                            ProductionStatus = prodStatus
                        });
                    }
                }

                // Sort by due date then order number
                result = result
                    .OrderBy(r => r.DueDate ?? DateTime.MaxValue)
                    .ThenBy(r => r.OrderNo)
                    .ToList();

                return ApiResponse<IEnumerable<ProductionOrderSummary>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductionOrderSummary>>.ErrorResponse($"Error loading production order items: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 2. Order detail — child parts + steps with schedule info
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<ProductionOrderDetail>> GetOrderDetailAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return ApiResponse<ProductionOrderDetail>.ErrorResponse("Order not found");

                var jobCards = (await _jobCardRepository.GetByOrderIdAsync(orderId)).ToList();
                if (!jobCards.Any())
                    return ApiResponse<ProductionOrderDetail>.ErrorResponse("No job cards found for this order");

                // Get OSP process IDs
                var uniqueProcessIds = jobCards.Select(jc => jc.ProcessId).Distinct();
                var ospProcessIds = new HashSet<int>();
                foreach (var pid in uniqueProcessIds)
                {
                    var process = await _processRepository.GetByIdAsync(pid);
                    if (process?.IsOutsourced == true)
                        ospProcessIds.Add(pid);
                }

                // Load schedules for all job cards (for machine info)
                var scheduleByJobCard = new Dictionary<int, (string? MachineName, string? MachineCode, DateTime? Start, DateTime? End, int? Duration)>();
                foreach (var jc in jobCards)
                {
                    var schedules = await _scheduleRepository.GetByJobCardIdAsync(jc.Id);
                    var active = schedules
                        .Where(s => s.Status == "Scheduled" || s.Status == "InProgress")
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefault();
                    if (active != null)
                        scheduleByJobCard[jc.Id] = (active.MachineName, active.MachineCode, active.ScheduledStartTime, active.ScheduledEndTime, active.EstimatedDurationMinutes);
                }

                // Only show job cards that have been scheduled (machine assigned)
                var scheduledCards = jobCards.Where(jc => jc.Status == "Scheduled").ToList();

                // Separate assembly from child-part steps
                var assemblyJcs = scheduledCards.Where(jc => jc.CreationType == "Assembly").ToList();
                var childJcs = scheduledCards.Where(jc => jc.CreationType != "Assembly").ToList();

                // Group child part steps by ChildPartKey (id when available, name otherwise)
                var childPartGroups = childJcs
                    .GroupBy(jc => ChildPartKey(jc))
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        var steps = g.OrderBy(jc => jc.StepNo ?? 999).ToList();
                        var completedSteps = steps.Count(jc => jc.ProductionStatus == "Completed");
                        var isReady = steps.All(jc => jc.ProductionStatus == "Completed");
                        var first = steps.First();

                        return new ProductionChildPartGroup
                        {
                            ChildPartId = first.ChildPartId,
                            ChildPartName = first.ChildPartName ?? "Unknown Part",
                            TotalSteps = steps.Count,
                            CompletedSteps = completedSteps,
                            IsReadyForAssembly = isReady,
                            Steps = steps.Select(jc => MapToStepItem(jc, scheduleByJobCard, ospProcessIds)).ToList()
                        };
                    }).ToList();

                // Assembly step (first one)
                ProductionStepItem? assemblyItem = null;
                if (assemblyJcs.Any())
                {
                    var asmJc = assemblyJcs.First();
                    assemblyItem = MapToStepItem(asmJc, scheduleByJobCard, ospProcessIds);
                }

                var allChildSteps = childJcs.Count;
                var completedChildSteps = childJcs.Count(jc => jc.ProductionStatus == "Completed");
                var inProgressChildSteps = childJcs.Count(jc => jc.ProductionStatus == "InProgress" || jc.ProductionStatus == "Paused");
                var canStartAssembly = childPartGroups.All(g => g.IsReadyForAssembly);

                var detail = new ProductionOrderDetail
                {
                    OrderId = order.Id,
                    OrderNo = order.OrderNo,
                    CustomerName = order.CustomerName,
                    ProductName = order.ProductName,
                    Priority = order.Priority ?? "Medium",
                    DueDate = order.DueDate,
                    TotalSteps = allChildSteps,
                    CompletedSteps = completedChildSteps,
                    InProgressSteps = inProgressChildSteps,
                    ChildParts = childPartGroups,
                    Assembly = assemblyItem,
                    CanStartAssembly = canStartAssembly
                };

                return ApiResponse<ProductionOrderDetail>.SuccessResponse(detail);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductionOrderDetail>.ErrorResponse($"Error loading order detail: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 2.5. Order ITEM detail — child parts + steps with schedule info (NEW)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<ProductionOrderDetail>> GetOrderItemDetailAsync(int orderItemId)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId);
                if (orderItem == null)
                    return ApiResponse<ProductionOrderDetail>.ErrorResponse("Order item not found");

                var order = await _orderRepository.GetByIdAsync(orderItem.OrderId);
                if (order == null)
                    return ApiResponse<ProductionOrderDetail>.ErrorResponse("Order not found");

                // Get job cards for this specific order item
                var jobCards = (await _jobCardRepository.GetByOrderItemIdAsync(orderItemId)).ToList();
                if (!jobCards.Any())
                    return ApiResponse<ProductionOrderDetail>.ErrorResponse("No job cards found for this order item");

                // Get OSP process IDs
                var uniqueProcessIds = jobCards.Select(jc => jc.ProcessId).Distinct();
                var ospProcessIds = new HashSet<int>();
                foreach (var pid in uniqueProcessIds)
                {
                    var process = await _processRepository.GetByIdAsync(pid);
                    if (process?.IsOutsourced == true)
                        ospProcessIds.Add(pid);
                }

                // Load schedules for all job cards (for machine info)
                var scheduleByJobCard = new Dictionary<int, (string? MachineName, string? MachineCode, DateTime? Start, DateTime? End, int? Duration)>();
                foreach (var jc in jobCards)
                {
                    var schedules = await _scheduleRepository.GetByJobCardIdAsync(jc.Id);
                    var active = schedules
                        .Where(s => s.Status == "Scheduled" || s.Status == "InProgress")
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefault();
                    if (active != null)
                        scheduleByJobCard[jc.Id] = (active.MachineName, active.MachineCode, active.ScheduledStartTime, active.ScheduledEndTime, active.EstimatedDurationMinutes);
                }

                // Only show job cards that have been scheduled (machine assigned)
                var scheduledCards = jobCards.Where(jc => jc.Status == "Scheduled").ToList();

                // Separate assembly from child-part steps
                var assemblyJcs = scheduledCards.Where(jc => jc.CreationType == "Assembly").ToList();
                var childJcs = scheduledCards.Where(jc => jc.CreationType != "Assembly").ToList();

                // Group child part steps by ChildPartKey (id when available, name otherwise)
                var childPartGroups = childJcs
                    .GroupBy(jc => ChildPartKey(jc))
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        var steps = g.OrderBy(jc => jc.StepNo ?? 999).ToList();
                        var completedSteps = steps.Count(jc => jc.ProductionStatus == "Completed");
                        var isReady = steps.All(jc => jc.ProductionStatus == "Completed");
                        var first = steps.First();

                        return new ProductionChildPartGroup
                        {
                            ChildPartId = first.ChildPartId,
                            ChildPartName = first.ChildPartName ?? "Unknown Part",
                            TotalSteps = steps.Count,
                            CompletedSteps = completedSteps,
                            IsReadyForAssembly = isReady,
                            Steps = steps.Select(jc => MapToStepItem(jc, scheduleByJobCard, ospProcessIds)).ToList()
                        };
                    }).ToList();

                // Assembly step (first one)
                ProductionStepItem? assemblyItem = null;
                if (assemblyJcs.Any())
                {
                    var asmJc = assemblyJcs.First();
                    assemblyItem = MapToStepItem(asmJc, scheduleByJobCard, ospProcessIds);
                }

                var allChildSteps = childJcs.Count;
                var completedChildSteps = childJcs.Count(jc => jc.ProductionStatus == "Completed");
                var inProgressChildSteps = childJcs.Count(jc => jc.ProductionStatus == "InProgress" || jc.ProductionStatus == "Paused");
                var canStartAssembly = childPartGroups.All(g => g.IsReadyForAssembly);

                var fullRef = !string.IsNullOrEmpty(orderItem.ItemSequence)
                    ? $"{order.OrderNo}-{orderItem.ItemSequence}"
                    : order.OrderNo;

                var detail = new ProductionOrderDetail
                {
                    OrderId = order.Id,
                    OrderNo = fullRef,
                    CustomerName = order.CustomerName,
                    ProductName = orderItem.ProductName,  // From OrderItem
                    Priority = orderItem.Priority ?? "Medium",
                    DueDate = orderItem.DueDate,
                    TotalSteps = allChildSteps,
                    CompletedSteps = completedChildSteps,
                    InProgressSteps = inProgressChildSteps,
                    ChildParts = childPartGroups,
                    Assembly = assemblyItem,
                    CanStartAssembly = canStartAssembly
                };

                return ApiResponse<ProductionOrderDetail>.SuccessResponse(detail);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductionOrderDetail>.ErrorResponse($"Error loading order item detail: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 3. Operator action: start | pause | resume | complete
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<bool>> HandleActionAsync(int jobCardId, ProductionActionRequest request)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var action = request.Action?.ToLower();
                string newStatus;
                DateTime? actualStart = jobCard.ActualStartTime;
                DateTime? actualEnd = jobCard.ActualEndTime;

                switch (action)
                {
                    case "start":
                        if (jobCard.ProductionStatus != "Ready")
                            return ApiResponse<bool>.ErrorResponse($"Cannot start: job card is '{jobCard.ProductionStatus}', expected 'Ready'");
                        newStatus = "InProgress";
                        actualStart = DateTime.UtcNow;
                        break;

                    case "pause":
                        if (jobCard.ProductionStatus != "InProgress")
                            return ApiResponse<bool>.ErrorResponse($"Cannot pause: job card is '{jobCard.ProductionStatus}', expected 'InProgress'");
                        newStatus = "Paused";
                        break;

                    case "resume":
                        if (jobCard.ProductionStatus != "Paused")
                            return ApiResponse<bool>.ErrorResponse($"Cannot resume: job card is '{jobCard.ProductionStatus}', expected 'Paused'");
                        newStatus = "InProgress";
                        break;

                    case "complete":
                        if (jobCard.ProductionStatus != "InProgress")
                            return ApiResponse<bool>.ErrorResponse($"Cannot complete: job card is '{jobCard.ProductionStatus}', expected 'InProgress'");
                        newStatus = "Completed";
                        actualEnd = DateTime.UtcNow;
                        break;

                    case "direct-complete":
                        // Allows completing from Ready, InProgress, or Paused in one shot
                        if (jobCard.ProductionStatus == "Ready")
                            actualStart = DateTime.UtcNow;
                        else if (jobCard.ProductionStatus != "InProgress" && jobCard.ProductionStatus != "Paused")
                            return ApiResponse<bool>.ErrorResponse($"Cannot complete: job card is '{jobCard.ProductionStatus}'");
                        newStatus = "Completed";
                        actualEnd = DateTime.UtcNow;
                        action = "complete"; // trigger cascade
                        break;

                    default:
                        return ApiResponse<bool>.ErrorResponse($"Unknown action '{request.Action}'. Use: start | pause | resume | complete");
                }

                // Update this job card
                await _jobCardRepository.UpdateProductionStatusAsync(
                    jobCardId,
                    newStatus,
                    actualStart,
                    actualEnd,
                    request.CompletedQty,
                    request.RejectedQty
                );

                // On complete — cascade to unlock next step + mark issued pieces as Consumed
                if (action == "complete")
                {
                    await CascadeOnCompleteAsync(jobCard);
                    await _pieceRepository.ConsumePiecesByJobCardAsync(jobCardId);
                }

                return ApiResponse<bool>.SuccessResponse(true, $"Job card {action}ed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error performing action: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cascade: when a step completes, unlock next step + check assembly
        // ─────────────────────────────────────────────────────────────────────
        private async Task CascadeOnCompleteAsync(Models.Planning.JobCard completedCard)
        {
            // Only cascade for non-assembly child part steps
            if (completedCard.CreationType == "Assembly") return;

            // For multi-product orders: only cascade within the same order item
            // For legacy orders: OrderItemId will be null, so we use OrderId
            var allOrderCards = completedCard.OrderItemId.HasValue
                ? (await _jobCardRepository.GetByOrderItemIdAsync(completedCard.OrderItemId.Value)).ToList()
                : (await _jobCardRepository.GetByOrderIdAsync(completedCard.OrderId)).ToList();

            var completedKey = ChildPartKey(completedCard);

            // Find the next step in the SAME child part group that is already Scheduled
            // (If not yet scheduled, ScheduleService will set it to Ready when machine is assigned)
            var nextStep = allOrderCards
                .Where(jc =>
                    ChildPartKey(jc) == completedKey &&
                    jc.CreationType != "Assembly" &&
                    jc.Status == "Scheduled" &&
                    jc.StepNo > completedCard.StepNo)
                .OrderBy(jc => jc.StepNo)
                .FirstOrDefault();

            if (nextStep != null && nextStep.ProductionStatus == "Pending")
            {
                // Unlock: mark next step as Ready
                await _jobCardRepository.UpdateProductionStatusAsync(
                    nextStep.Id, "Ready",
                    nextStep.ActualStartTime, nextStep.ActualEndTime,
                    nextStep.CompletedQty, nextStep.RejectedQty
                );
            }

            // Check if ALL steps of this child part are now Completed
            var childPartSteps = allOrderCards
                .Where(jc => ChildPartKey(jc) == completedKey && jc.CreationType != "Assembly")
                .ToList();

            // Re-fetch the completed step so its status is updated
            var updatedCompleted = await _jobCardRepository.GetByIdAsync(completedCard.Id);
            if (updatedCompleted == null) return;

            // Replace in list for accurate check
            var freshList = childPartSteps
                .Select(jc => jc.Id == updatedCompleted.Id ? updatedCompleted : jc)
                .ToList();

            bool allDone = freshList.All(jc => jc.ProductionStatus == "Completed");

            // Mark ReadyForAssembly for all steps of this child part
            foreach (var jc in childPartSteps)
            {
                await _jobCardRepository.SetReadyForAssemblyAsync(jc.Id, allDone);
            }

            if (allDone)
            {
                // Check if ALL child parts across the order are done
                var allChildCards = allOrderCards.Where(jc => jc.CreationType != "Assembly").ToList();

                // Re-fetch to get latest status
                var freshChildCards = new List<Models.Planning.JobCard>();
                foreach (var jc in allChildCards)
                {
                    var fresh = await _jobCardRepository.GetByIdAsync(jc.Id);
                    if (fresh != null) freshChildCards.Add(fresh);
                }

                bool allChildPartsDone = freshChildCards.All(jc => jc.ReadyForAssembly || jc.ProductionStatus == "Completed");

                if (allChildPartsDone)
                {
                    // Unlock assembly step only if it has been scheduled (machine assigned)
                    var assemblyCard = allOrderCards.FirstOrDefault(jc =>
                        jc.CreationType == "Assembly" && jc.Status == "Scheduled");
                    if (assemblyCard != null && assemblyCard.ProductionStatus == "Pending")
                    {
                        await _jobCardRepository.UpdateProductionStatusAsync(
                            assemblyCard.Id, "Ready",
                            assemblyCard.ActualStartTime, assemblyCard.ActualEndTime,
                            assemblyCard.CompletedQty, assemblyCard.RejectedQty
                        );
                    }
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 4. Process-based Execution View — ProcessCategory → ChildPart → Orders
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<IEnumerable<ExecutionViewCategory>>> GetExecutionViewAsync()
        {
            try
            {
                // All scheduled, non-assembly job cards
                var allScheduled = (await _jobCardRepository.GetByStatusAsync("Scheduled"))
                    .Where(jc => jc.CreationType != "Assembly")
                    .ToList();

                if (!allScheduled.Any())
                    return ApiResponse<IEnumerable<ExecutionViewCategory>>.SuccessResponse(new List<ExecutionViewCategory>());

                // Cache process → category info (one DB call per unique process)
                var processCache = new Dictionary<int, Models.Masters.Process>();
                foreach (var pid in allScheduled.Select(jc => jc.ProcessId).Distinct())
                {
                    var proc = await _processRepository.GetByIdAsync(pid);
                    if (proc != null) processCache[pid] = proc;
                }

                // Cache OSP status per job card (only "Sent" entries)
                var ospStatusMap = await _ospRepository.GetActiveOspStatusByJobCardIdsAsync(
                    allScheduled.Select(jc => jc.Id));

                // Cache schedule → machine name
                var machineCache = new Dictionary<int, string?>();
                foreach (var jc in allScheduled)
                {
                    var schedules = await _scheduleRepository.GetByJobCardIdAsync(jc.Id);
                    var active = schedules
                        .Where(s => s.Status == "Scheduled" || s.Status == "InProgress")
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefault();
                    machineCache[jc.Id] = active?.MachineName;
                }

                // Group by ProcessCategory
                var categoryGroups = allScheduled
                    .GroupBy(jc =>
                    {
                        processCache.TryGetValue(jc.ProcessId, out var p);
                        return new
                        {
                            CategoryId = p?.ProcessCategoryId,
                            CategoryName = p?.ProcessCategoryName ?? "Uncategorized",
                            CategoryCode = (string?)null
                        };
                    })
                    .OrderBy(g => g.Key.CategoryName != null && g.Key.CategoryName.Contains("Assembly") ? 1 : 0)
                    .ThenBy(g => g.Key.CategoryName)
                    .Select(categoryGroup =>
                    {
                        // Group by child part within this category
                        var childPartGroups = categoryGroup
                            .GroupBy(jc => ChildPartKey(jc))
                            .OrderBy(g => g.Key)
                            .Select(childGroup =>
                            {
                                var rows = childGroup
                                    .OrderBy(jc => jc.OrderNo)
                                    .ThenBy(jc => jc.StepNo)
                                    .Select(jc =>
                                    {
                                        bool isLocked = jc.ProductionStatus == "Pending";
                                        string? waitingFor = null;

                                        if (isLocked)
                                        {
                                            // Find the step blocking this one (same order + child part, lower step, not completed)
                                            var blockingStep = allScheduled
                                                .Where(other =>
                                                    other.OrderId == jc.OrderId &&
                                                    ChildPartKey(other) == ChildPartKey(jc) &&
                                                    other.StepNo < jc.StepNo &&
                                                    other.ProductionStatus != "Completed")
                                                .OrderByDescending(other => other.StepNo)
                                                .FirstOrDefault();
                                            waitingFor = blockingStep?.ProcessName;
                                        }

                                        machineCache.TryGetValue(jc.Id, out var machineName);
                                        processCache.TryGetValue(jc.ProcessId, out var procInfo);
                                        ospStatusMap.TryGetValue(jc.Id, out var ospStatus);

                                        return new ExecutionViewRow
                                        {
                                            JobCardId = jc.Id,
                                            JobCardNo = jc.JobCardNo,
                                            OrderId = jc.OrderId,
                                            OrderItemId = jc.OrderItemId,
                                            OrderNo = !string.IsNullOrEmpty(jc.ItemSequence)
                                                ? $"{jc.OrderNo}-{jc.ItemSequence}"
                                                : (jc.OrderNo ?? jc.OrderId.ToString()),
                                            ChildPartName = jc.ChildPartName,
                                            ProcessName = jc.ProcessName,
                                            StepNo = jc.StepNo,
                                            Quantity = jc.Quantity,
                                            CompletedQty = jc.CompletedQty,
                                            RejectedQty = jc.RejectedQty,
                                            ProductionStatus = jc.ProductionStatus,
                                            IsLocked = isLocked,
                                            WaitingFor = waitingFor,
                                            MachineName = machineName,
                                            ActualStartTime = jc.ActualStartTime,
                                            ActualEndTime = jc.ActualEndTime,
                                            IsOsp = procInfo?.IsOutsourced ?? false,
                                            OspStatus = ospStatus,  // "Sent" or null
                                        };
                                    }).ToList();

                                var first = childGroup.First();
                                bool isReadyForAssembly = childGroup.All(jc => jc.ReadyForAssembly || jc.ProductionStatus == "Completed");

                                return new ExecutionViewChildPart
                                {
                                    ChildPartName = first.ChildPartName ?? "Unknown Part",
                                    ChildPartId = first.ChildPartId,
                                    IsReadyForAssembly = isReadyForAssembly,
                                    JobCards = rows
                                };
                            }).ToList();

                        var key = categoryGroup.Key;
                        return new ExecutionViewCategory
                        {
                            ProcessCategoryId = key.CategoryId,
                            CategoryName = key.CategoryName,
                            CategoryCode = key.CategoryCode,
                            TotalJobs = categoryGroup.Count(),
                            ReadyJobs = categoryGroup.Count(jc => jc.ProductionStatus == "Ready"),
                            InProgressJobs = categoryGroup.Count(jc => jc.ProductionStatus == "InProgress" || jc.ProductionStatus == "Paused"),
                            CompletedJobs = categoryGroup.Count(jc => jc.ProductionStatus == "Completed"),
                            ChildParts = childPartGroups
                        };
                    }).ToList();

                return ApiResponse<IEnumerable<ExecutionViewCategory>>.SuccessResponse(categoryGroups);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ExecutionViewCategory>>.ErrorResponse($"Error loading execution view: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helper
        // ─────────────────────────────────────────────────────────────────────
        private static ProductionStepItem MapToStepItem(
            Models.Planning.JobCard jc,
            Dictionary<int, (string? MachineName, string? MachineCode, DateTime? Start, DateTime? End, int? Duration)> scheduleMap,
            HashSet<int> ospProcessIds)
        {
            scheduleMap.TryGetValue(jc.Id, out var sched);
            return new ProductionStepItem
            {
                JobCardId = jc.Id,
                JobCardNo = jc.JobCardNo,
                StepNo = jc.StepNo,
                ProcessName = jc.ProcessName,
                ProcessCode = jc.ProcessCode,
                IsOsp = ospProcessIds.Contains(jc.ProcessId),
                ProductionStatus = jc.ProductionStatus,
                ActualStartTime = jc.ActualStartTime,
                ActualEndTime = jc.ActualEndTime,
                Quantity = jc.Quantity,
                CompletedQty = jc.CompletedQty,
                RejectedQty = jc.RejectedQty,
                ScheduledStartTime = sched.Start,
                ScheduledEndTime = sched.End,
                MachineName = sched.MachineName,
                MachineCode = sched.MachineCode,
                EstimatedDurationMinutes = sched.Duration
            };
        }
    }
}
