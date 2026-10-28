using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.WorkOrders.Commands.AddDocument;

public record AddWorkOrderDocumentCommand(
    Guid WorkOrderId,
    string Name,
    string Type,
    string FileUrl
) : IRequest<Result<WorkOrderDocumentDto>>;
