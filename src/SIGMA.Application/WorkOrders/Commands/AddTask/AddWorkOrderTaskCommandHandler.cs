using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.WorkOrders.Commands.AddTask;

public class AddWorkOrderTaskCommandHandler : IRequestHandler<AddWorkOrderTaskCommand, Result<WorkOrderTaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddWorkOrderTaskCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderTaskDto>> Handle(AddWorkOrderTaskCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Tasks)
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (workOrder is null)
            return Result<WorkOrderTaskDto>.Failure("Orden de trabajo no encontrada.");

        var orderIndex = workOrder.Tasks.Count + 1;
        var task = WorkOrderTask.Create(
            workOrder.Id, orderIndex, request.Title,
            request.Description, request.EstimatedHours, request.RequiresInspection);

        _context.WorkOrderTasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<WorkOrderTaskDto>.Success(_mapper.Map<WorkOrderTaskDto>(task));
    }
}
