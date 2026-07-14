using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.WorkOrders.Commands.AddMaterial;

// Maneja el alta de un material/repuesto pedido para una OT, mismo patron que AddWorkOrderTaskCommandHandler
public class AddWorkOrderMaterialCommandHandler : IRequestHandler<AddWorkOrderMaterialCommand, Result<WorkOrderMaterialDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddWorkOrderMaterialCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderMaterialDto>> Handle(AddWorkOrderMaterialCommand request, CancellationToken cancellationToken)
    {
        var workOrderExists = await _context.WorkOrders
            .AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (!workOrderExists)
            return Result<WorkOrderMaterialDto>.Failure("Orden de trabajo no encontrada.");

        var material = WorkOrderMaterial.Create(
            request.WorkOrderId, request.PartNumber, request.Description, request.Quantity, request.Unit);

        _context.WorkOrderMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<WorkOrderMaterialDto>.Success(_mapper.Map<WorkOrderMaterialDto>(material));
    }
}
