using SIGMA.Domain.Common;

namespace SIGMA.Domain.Events;

public record WorkOrderCreatedEvent(Guid WorkOrderId, string Number, Guid CreatedById) : IDomainEvent;
