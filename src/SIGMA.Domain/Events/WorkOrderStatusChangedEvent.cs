using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;

namespace SIGMA.Domain.Events;

public record WorkOrderStatusChangedEvent(
    Guid WorkOrderId,
    WorkOrderStatus OldStatus,
    WorkOrderStatus NewStatus,
    Guid ChangedById) : IDomainEvent;
