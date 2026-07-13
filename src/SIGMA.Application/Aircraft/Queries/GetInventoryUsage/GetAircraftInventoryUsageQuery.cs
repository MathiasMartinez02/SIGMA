using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Queries.GetInventoryUsage;

// Consulta la trazabilidad de repuestos usados en OTs de una aeronave especifica
public record GetAircraftInventoryUsageQuery(Guid AircraftId) : IRequest<Result<IList<AircraftInventoryUsageDto>>>;
