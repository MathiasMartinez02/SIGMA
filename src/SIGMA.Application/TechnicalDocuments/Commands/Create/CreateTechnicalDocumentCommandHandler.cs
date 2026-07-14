using MediatR;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.TechnicalDocuments.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.TechnicalDocuments.Commands.Create;

public class CreateTechnicalDocumentCommandHandler : IRequestHandler<CreateTechnicalDocumentCommand, Result<TechnicalDocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateTechnicalDocumentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<TechnicalDocumentDto>> Handle(CreateTechnicalDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = TechnicalDocument.Create(
            request.Type,
            request.Title,
            request.FileUrl,
            request.ReferenceCode,
            request.Description,
            request.IssueDate,
            request.ExpiryDate);

        _context.TechnicalDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new TechnicalDocumentDto
        {
            Id = document.Id,
            Type = document.Type,
            Title = document.Title,
            ReferenceCode = document.ReferenceCode,
            FileUrl = document.FileUrl,
            Description = document.Description,
            IssueDate = document.IssueDate,
            ExpiryDate = document.ExpiryDate,
            IsExpired = document.IsExpired,
            CreatedAt = document.CreatedAt
        };

        return Result<TechnicalDocumentDto>.Success(dto);
    }
}
