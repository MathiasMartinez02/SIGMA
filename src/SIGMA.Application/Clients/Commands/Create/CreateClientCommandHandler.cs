using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Clients.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Clients.Commands.Create;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<ClientDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateClientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClientDto>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var taxIdExists = await _context.Clients
            .AnyAsync(c => c.TaxId == request.TaxId && !c.IsDeleted, cancellationToken);

        if (taxIdExists)
            return Result<ClientDto>.Failure($"Ya existe un cliente con el CUIT '{request.TaxId}'.");

        var client = Client.Create(
            request.Name,
            request.BusinessName,
            request.TaxId,
            request.Email,
            request.Phone,
            request.Address,
            request.City,
            request.Province,
            request.ContactPerson,
            request.ContactPhone);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            BusinessName = client.BusinessName,
            TaxId = client.TaxId,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            City = client.City,
            Province = client.Province,
            ContactPerson = client.ContactPerson,
            ContactPhone = client.ContactPhone,
            IsActive = client.IsActive,
            AircraftCount = 0,
            TotalWorkOrders = 0,
            CreatedAt = client.CreatedAt
        };

        return Result<ClientDto>.Success(dto);
    }
}
