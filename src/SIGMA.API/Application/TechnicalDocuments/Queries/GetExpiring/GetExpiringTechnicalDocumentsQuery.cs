using MediatR;
using SIGMA.API.TechnicalDocuments.DTOs;

namespace SIGMA.API.TechnicalDocuments.Queries.GetExpiring;

// Query independiente del Dashboard para consultar documentos tecnicos por vencer (ventana de 30 dias, mismo criterio que NotificationGeneratorService)
public record GetExpiringTechnicalDocumentsQuery : IRequest<IReadOnlyList<TechnicalDocumentDto>>;
