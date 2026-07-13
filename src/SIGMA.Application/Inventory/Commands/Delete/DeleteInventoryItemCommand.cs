using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inventory.Commands.Delete;

// Comando que da de baja (soft delete) un item de inventario por su id
public record DeleteInventoryItemCommand(Guid Id) : IRequest<Result>;
