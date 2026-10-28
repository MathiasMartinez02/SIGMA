using MediatR;
using SIGMA.API.Reports.DTOs;

namespace SIGMA.API.Reports.Queries.Inventory;

public record GetInventoryReportQuery() : IRequest<InventoryReportDto>;
