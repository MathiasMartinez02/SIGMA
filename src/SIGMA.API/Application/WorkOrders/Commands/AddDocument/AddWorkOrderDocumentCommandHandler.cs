using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Entities;
using SIGMA.API.WorkOrders.Services;

namespace SIGMA.API.WorkOrders.Commands.AddDocument;

public class AddWorkOrderDocumentCommandHandler : IRequestHandler<AddWorkOrderDocumentCommand, Result<WorkOrderDocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddWorkOrderDocumentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WorkOrderDocumentDto>> Handle(AddWorkOrderDocumentCommand request, CancellationToken cancellationToken)
    {
        var (workOrder, error) = await EditableWorkOrder.FindAsync(_context, request.WorkOrderId, cancellationToken);

        if (workOrder is null)
            return Result<WorkOrderDocumentDto>.Failure(error!);

        var uploadedById = _currentUser.UserId!.Value;

        var document = WorkOrderDocument.Create(request.WorkOrderId, request.Name, request.Type, request.FileUrl, uploadedById);

        _context.WorkOrderDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new WorkOrderDocumentDto
        {
            Id = document.Id,
            Name = document.Name,
            Type = document.Type,
            FileUrl = document.FileUrl,
            UploadedById = document.UploadedById,
            UploadedAt = document.UploadedAt
        };

        return Result<WorkOrderDocumentDto>.Success(dto);
    }
}
