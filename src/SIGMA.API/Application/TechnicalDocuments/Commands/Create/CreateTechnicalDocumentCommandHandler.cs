using MediatR;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.TechnicalDocuments.DTOs;
using SIGMA.API.Entities;

namespace SIGMA.API.TechnicalDocuments.Commands.Create;

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
