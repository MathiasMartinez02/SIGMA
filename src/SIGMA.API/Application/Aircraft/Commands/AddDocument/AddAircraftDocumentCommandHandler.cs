using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Entities;

namespace SIGMA.API.Aircraft.Commands.AddDocument;

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
