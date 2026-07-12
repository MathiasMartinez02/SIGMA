using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inspections.DTOs;

namespace SIGMA.Application.Inspections.Queries.GetById;

public class GetInspectionByIdQueryHandler : IRequestHandler<GetInspectionByIdQuery, Result<InspectionDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInspectionByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<InspectionDetailDto>> Handle(GetInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.ChecklistSections.OrderBy(s => s.OrderIndex))
                .ThenInclude(s => s.Items.OrderBy(item => item.OrderIndex))
            .Include(i => i.WorkOrder)
            .Include(i => i.Aircraft)
            .Include(i => i.Inspector)
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result<InspectionDetailDto>.Failure("La inspección no fue encontrada.");

        var dto = _mapper.Map<InspectionDetailDto>(inspection);
        return Result<InspectionDetailDto>.Success(dto);
    }
}
