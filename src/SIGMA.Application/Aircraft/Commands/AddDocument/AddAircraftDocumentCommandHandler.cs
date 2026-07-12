using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Aircraft.Commands.AddDocument;

public class AddAircraftDocumentCommandHandler : IRequestHandler<AddAircraftDocumentCommand, Result<AircraftDocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public AddAircraftDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AircraftDocumentDto>> Handle(AddAircraftDocumentCommand request, CancellationToken cancellationToken)
    {
        var aircraftExists = await _context.Aircraft
            .AnyAsync(a => a.Id == request.AircraftId && !a.IsDeleted, cancellationToken);

        if (!aircraftExists)
            return Result<AircraftDocumentDto>.Failure("La aeronave no fue encontrada.");

        var document = AircraftDocument.Create(
            request.AircraftId,
            request.Type,
            request.Name,
            request.FileUrl,
            request.ExpiryDate);

        _context.AircraftDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new AircraftDocumentDto
        {
            Id = document.Id,
            Type = document.Type,
            Name = document.Name,
            FileUrl = document.FileUrl,
            ExpiryDate = document.ExpiryDate,
            IsExpired = document.IsExpired,
            UploadedAt = document.UploadedAt
        };

        return Result<AircraftDocumentDto>.Success(dto);
    }
}
