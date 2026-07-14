using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.TechnicalDocuments.DTOs;

namespace SIGMA.Application.TechnicalDocuments.Queries.GetById;

public class GetTechnicalDocumentByIdQueryHandler : IRequestHandler<GetTechnicalDocumentByIdQuery, Result<TechnicalDocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTechnicalDocumentByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<TechnicalDocumentDto>> Handle(GetTechnicalDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.TechnicalDocuments
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (document is null)
            return Result<TechnicalDocumentDto>.Failure("El documento tecnico no fue encontrado.");

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
