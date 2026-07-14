using MediatR;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.TechnicalDocuments.DTOs;

namespace SIGMA.Application.TechnicalDocuments.Queries.GetAll;

public class GetTechnicalDocumentsQueryHandler : IRequestHandler<GetTechnicalDocumentsQuery, PaginatedResult<TechnicalDocumentDto>>
{
    // Misma ventana de 30 dias que usa NotificationGeneratorService para "documento por vencer", para no desalinear criterios de negocio
    private static readonly TimeSpan ExpiryWindow = TimeSpan.FromDays(30);

    private readonly IApplicationDbContext _context;

    public GetTechnicalDocumentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<TechnicalDocumentDto>> Handle(GetTechnicalDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TechnicalDocuments
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        if (request.Type.HasValue)
            query = query.Where(d => d.Type == request.Type.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(d =>
                d.Title.ToLower().Contains(search) ||
                (d.ReferenceCode != null && d.ReferenceCode.ToLower().Contains(search)));
        }

        if (request.ExpiringOnly == true)
        {
            var now = DateTime.UtcNow;
            var threshold = now.Add(ExpiryWindow);
            query = query.Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value >= now && d.ExpiryDate.Value <= threshold);
        }

        query = query.OrderBy(d => d.ExpiryDate == null).ThenBy(d => d.ExpiryDate).ThenByDescending(d => d.CreatedAt);

        var mapped = query.Select(d => new TechnicalDocumentDto
        {
            Id = d.Id,
            Type = d.Type,
            Title = d.Title,
            ReferenceCode = d.ReferenceCode,
            FileUrl = d.FileUrl,
            Description = d.Description,
            IssueDate = d.IssueDate,
            ExpiryDate = d.ExpiryDate,
            IsExpired = d.IsExpired,
            CreatedAt = d.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
