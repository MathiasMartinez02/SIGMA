using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Enums;
using SIGMA.API.Inspections.Commands.Approve;
using SIGMA.API.Inspections.Commands.Complete;
using SIGMA.API.Inspections.Commands.Reject;
using SIGMA.API.Inspections.Commands.Start;
using SIGMA.API.Inspections.Commands.UpdateChecklistItem;
using SIGMA.API.Persistence;
using SIGMA.API.WorkOrders.Commands.UpdateStatus;
using Xunit;

namespace SIGMA.Tests.Application;

public class InspectionFlowTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options =
        new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

    private readonly ICurrentUserService _user = Substitute.For<ICurrentUserService>();

    public InspectionFlowTests()
    {
        _user.UserId.Returns(Guid.NewGuid());
        _user.FullName.Returns("Maria Gonzalez");
        _user.Role.Returns("Inspector");
    }

    // Contexto nuevo por paso, como en requests HTTP separados
    private ApplicationDbContext Context() => new(_options);

    private async Task ChangeStatus(Guid workOrderId, WorkOrderStatus status, bool expectSuccess = true)
    {
        await using var ctx = Context();
        var result = await new UpdateWorkOrderStatusCommandHandler(ctx, _user)
            .Handle(new UpdateWorkOrderStatusCommand(workOrderId, status), CancellationToken.None);
        result.Succeeded.Should().Be(expectSuccess, string.Join(", ", result.Errors));
    }

    private async Task<Guid> OpenInspectionId(Guid workOrderId)
    {
        await using var ctx = Context();
        return await ctx.Inspections.Where(i => i.WorkOrderId == workOrderId)
            .OrderByDescending(i => i.CreatedAt).Select(i => i.Id).FirstAsync();
    }

    private async Task PassInspection(Guid inspectionId, ChecklistItemStatus itemStatus = ChecklistItemStatus.Conforme)
    {
        await using (var ctx = Context())
            (await new StartInspectionCommandHandler(ctx, _user).Handle(new StartInspectionCommand(inspectionId), default))
                .Succeeded.Should().BeTrue();

        List<Guid> itemIds;
        await using (var ctx = Context())
            itemIds = await ctx.Inspections.Where(i => i.Id == inspectionId)
                .SelectMany(i => i.ChecklistSections.SelectMany(s => s.Items).Select(it => it.Id)).ToListAsync();

        foreach (var itemId in itemIds)
        {
            await using var ctx = Context();
            var result = await new UpdateChecklistItemCommandHandler(ctx, _user)
                .Handle(new UpdateChecklistItemCommand(inspectionId, itemId, itemStatus, "Controlado"), default);
            result.Succeeded.Should().BeTrue(string.Join(", ", result.Errors));
        }

        await using (var ctx = Context())
            (await new CompleteInspectionCommandHandler(ctx).Handle(new CompleteInspectionCommand(inspectionId, null), default))
                .Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task OT_de_inspeccion_100h_de_punta_a_punta()
    {
        Guid workOrderId, aircraftId;
        await using (var ctx = Context())
        {
            var (_, aircraft, workOrder) = await TestDb.SeedWorkOrderAsync(ctx, DateTime.UtcNow);
            workOrderId = workOrder.Id;
            aircraftId = aircraft.Id;
        }

        await ChangeStatus(workOrderId, WorkOrderStatus.EnProceso);
        await using (var ctx = Context())
            (await ctx.Aircraft.FindAsync(aircraftId))!.Status.Should().Be(AircraftStatus.EnMantenimiento);

        await ChangeStatus(workOrderId, WorkOrderStatus.EnInspeccion);
        await ChangeStatus(workOrderId, WorkOrderStatus.Finalizada, expectSuccess: false);

        var inspectionId = await OpenInspectionId(workOrderId);
        await PassInspection(inspectionId);

        await using (var ctx = Context())
            (await new ApproveInspectionCommandHandler(ctx, _user).Handle(new ApproveInspectionCommand(inspectionId, null), default))
                .Succeeded.Should().BeTrue();

        await ChangeStatus(workOrderId, WorkOrderStatus.Finalizada);

        await using (var ctx = Context())
        {
            var aircraft = await ctx.Aircraft.FindAsync(aircraftId);
            aircraft!.Status.Should().Be(AircraftStatus.Operativa);
            aircraft.LastInspectionDate.Should().NotBeNull();
            aircraft.NextInspectionHours.Should().Be(aircraft.TotalFlightHours + 100m);
            (await ctx.Inspections.CountAsync(i => i.WorkOrderId == workOrderId)).Should().Be(1);
        }
    }

    [Fact]
    public async Task Rechazo_devuelve_la_OT_a_proceso_y_la_reinspeccion_crea_una_nueva()
    {
        Guid workOrderId, aircraftId;
        await using (var ctx = Context())
        {
            var (_, aircraft, workOrder) = await TestDb.SeedWorkOrderAsync(ctx, DateTime.UtcNow);
            workOrderId = workOrder.Id;
            aircraftId = aircraft.Id;
        }

        await ChangeStatus(workOrderId, WorkOrderStatus.EnProceso);
        await ChangeStatus(workOrderId, WorkOrderStatus.EnInspeccion);
        var firstInspection = await OpenInspectionId(workOrderId);
        await PassInspection(firstInspection, ChecklistItemStatus.NoConforme);

        await using (var ctx = Context())
            (await new RejectInspectionCommandHandler(ctx, _user)
                .Handle(new RejectInspectionCommand(firstInspection, "Pérdida de aceite en carter"), default))
                .Succeeded.Should().BeTrue();

        await using (var ctx = Context())
        {
            (await ctx.WorkOrders.FindAsync(workOrderId))!.Status.Should().Be(WorkOrderStatus.EnProceso);
            (await ctx.Aircraft.FindAsync(aircraftId))!.Status.Should().Be(AircraftStatus.EnMantenimiento);
        }

        await ChangeStatus(workOrderId, WorkOrderStatus.EnInspeccion);

        await using (var ctx = Context())
            (await ctx.Inspections.CountAsync(i => i.WorkOrderId == workOrderId)).Should().Be(2);
    }
}
