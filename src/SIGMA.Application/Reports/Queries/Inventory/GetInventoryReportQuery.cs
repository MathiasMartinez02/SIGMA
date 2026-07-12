using MediatR;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.Inventory;

public record GetInventoryReportQuery() : IRequest<InventoryReportDto>;
