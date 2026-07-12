using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Queries.GetById;

public class GetAircraftByIdQueryHandler : IRequestHandler<GetAircraftByIdQuery, Result<AircraftDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAircraftByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<AircraftDetailDto>> Handle(GetAircraftByIdQuery request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .Include(a => a.Documents)
            .Include(a => a.FlightRecords)
            .Include(a => a.Components)
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result<AircraftDetailDto>.Failure("La aeronave no fue encontrada.");

        var dto = _mapper.Map<AircraftDetailDto>(aircraft);
        return Result<AircraftDetailDto>.Success(dto);
    }
}
