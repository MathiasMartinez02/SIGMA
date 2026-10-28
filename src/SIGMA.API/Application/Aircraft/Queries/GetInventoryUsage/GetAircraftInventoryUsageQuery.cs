using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Aircraft.Queries.GetInventoryUsage;

// Consulta la trazabilidad de repuestos usados en OTs de una aeronave especifica
public record GetAircraftInventoryUsageQuery(Guid AircraftId) : IRequest<Result<IList<AircraftInventoryUsageDto>>>;
