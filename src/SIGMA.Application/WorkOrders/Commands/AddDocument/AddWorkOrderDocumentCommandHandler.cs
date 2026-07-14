using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.WorkOrders.Commands.AddDocument;

// Handler que faltaba: WorkOrderDocument existia en el Domain pero no tenia Command/Controller que lo expusiera (Fase 4)
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
        var workOrderExists = await _context.WorkOrders
            .AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (!workOrderExists)
            return Result<WorkOrderDocumentDto>.Failure("La orden de trabajo no fue encontrada.");

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
