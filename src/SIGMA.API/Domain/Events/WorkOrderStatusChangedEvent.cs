using SIGMA.API.Common;
using SIGMA.API.Enums;

namespace SIGMA.API.Events;

public record WorkOrderStatusChangedEvent(
    Guid WorkOrderId,
    WorkOrderStatus OldStatus,
    WorkOrderStatus NewStatus,
    Guid ChangedById) : IDomainEvent;
