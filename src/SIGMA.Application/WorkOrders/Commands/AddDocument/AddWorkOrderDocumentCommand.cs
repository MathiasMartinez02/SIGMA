using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.WorkOrders.Commands.AddDocument;

public record AddWorkOrderDocumentCommand(
    Guid WorkOrderId,
    string Name,
    string Type,
    string FileUrl
) : IRequest<Result<WorkOrderDocumentDto>>;
