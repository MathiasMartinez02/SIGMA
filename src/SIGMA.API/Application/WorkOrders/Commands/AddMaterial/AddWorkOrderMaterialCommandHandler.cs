using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Entities;
using SIGMA.API.WorkOrders.Services;

namespace SIGMA.API.WorkOrders.Commands.AddMaterial;

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
        var (workOrder, error) = await EditableWorkOrder.FindAsync(_context, request.WorkOrderId, cancellationToken);

        if (workOrder is null)
            return Result<WorkOrderMaterialDto>.Failure(error!);

        var material = WorkOrderMaterial.Create(
            request.WorkOrderId, request.PartNumber, request.Description, request.Quantity, request.Unit);

        _context.WorkOrderMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<WorkOrderMaterialDto>.Success(_mapper.Map<WorkOrderMaterialDto>(material));
    }
}
