using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.DTOs;

namespace SIGMA.Application.Inventory.Queries.GetById;

public record GetInventoryItemByIdQuery(Guid Id) : IRequest<Result<InventoryItemDto>>;
