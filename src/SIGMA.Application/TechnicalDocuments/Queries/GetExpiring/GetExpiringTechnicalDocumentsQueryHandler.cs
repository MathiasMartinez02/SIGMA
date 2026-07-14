using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.TechnicalDocuments.DTOs;

namespace SIGMA.Application.TechnicalDocuments.Queries.GetExpiring;

public class GetExpiringTechnicalDocumentsQueryHandler : IRequestHandler<GetExpiringTechnicalDocumentsQuery, IReadOnlyList<TechnicalDocumentDto>>
{
    // Misma ventana de 30 dias usada en NotificationGeneratorService.NotifyAircraftDocumentsAsync, para mantener el mismo criterio de negocio
    private static readonly TimeSpan ExpiryWindow = TimeSpan.FromDays(30);

    private readonly IApplicationDbContext _context;

    public GetExpiringTechnicalDocumentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<TechnicalDocumentDto>> Handle(GetExpiringTechnicalDocumentsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var threshold = now.Add(ExpiryWindow);

        var documents = await _context.TechnicalDocuments
            .Where(d => !d.IsDeleted && d.ExpiryDate.HasValue && d.ExpiryDate.Value >= now && d.ExpiryDate.Value <= threshold)
            .OrderBy(d => d.ExpiryDate)
            .Select(d => new TechnicalDocumentDto
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
            })
            .ToListAsync(cancellationToken);

        return documents;
    }
}
