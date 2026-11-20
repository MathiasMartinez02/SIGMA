using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Entities;
using SIGMA.API.WorkOrders.Services;

namespace SIGMA.API.WorkOrders.Commands.AddTask;

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
        var (workOrder, error) = await EditableWorkOrder.FindAsync(
            _context, request.WorkOrderId, cancellationToken, q => q.Include(w => w.Tasks));

        if (workOrder is null)
            return Result<WorkOrderTaskDto>.Failure(error!);

        var orderIndex = workOrder.Tasks.Count + 1;
        var task = WorkOrderTask.Create(
            workOrder.Id, orderIndex, request.Title,
            request.Description, request.EstimatedHours, request.RequiresInspection);

        _context.WorkOrderTasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<WorkOrderTaskDto>.Success(_mapper.Map<WorkOrderTaskDto>(task));
    }
}
