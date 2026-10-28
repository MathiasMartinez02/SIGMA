using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Inventory.DTOs;

namespace SIGMA.API.Inventory.Queries.GetById;

public record GetInventoryItemByIdQuery(Guid Id) : IRequest<Result<InventoryItemDto>>;
