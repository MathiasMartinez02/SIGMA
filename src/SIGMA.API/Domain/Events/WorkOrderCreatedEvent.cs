using SIGMA.API.Common;

namespace SIGMA.API.Events;

public record WorkOrderCreatedEvent(Guid WorkOrderId, string Number, Guid CreatedById) : IDomainEvent;
