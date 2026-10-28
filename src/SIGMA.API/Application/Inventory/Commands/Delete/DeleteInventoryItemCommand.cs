using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inventory.Commands.Delete;

// Comando que da de baja (soft delete) un item de inventario por su id
public record DeleteInventoryItemCommand(Guid Id) : IRequest<Result>;
