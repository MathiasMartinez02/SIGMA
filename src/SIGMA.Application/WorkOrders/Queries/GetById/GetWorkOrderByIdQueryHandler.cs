using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.WorkOrders.Queries.GetById;

public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetWorkOrderByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderDetailDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        // MODIFICADO: se agrega AsSplitQuery(). Con 4 colecciones hermanas (Tasks, Timeline, Materials, Documents,
        // AssignedMechanics) incluidas en una sola consulta, EF Core arma un JOIN cartesiano que puede devolver
        // filas duplicadas o mal agrupadas (mismo problema detectado y corregido en UpdateWorkOrderStatusCommandHandler).
        // AsSplitQuery separa cada colección en su propia consulta, evitando resultados incorrectos en el detalle de la OT.
        // ANTERIOR: sin AsSplitQuery()
        var workOrder = await _context.WorkOrders
            .Include(w => w.Aircraft)
            .Include(w => w.Client)
            .Include(w => w.Tasks).ThenInclude(t => t.AssignedTo)
            .Include(w => w.Tasks).ThenInclude(t => t.InspectedBy)
            .Include(w => w.Timeline)
            .Include(w => w.Materials)
            .Include(w => w.Documents)
            .Include(w => w.AssignedMechanics).ThenInclude(am => am.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (workOrder is null)
            return Result<WorkOrderDetailDto>.Failure("Orden de trabajo no encontrada.");

        return Result<WorkOrderDetailDto>.Success(_mapper.Map<WorkOrderDetailDto>(workOrder));
    }
}
