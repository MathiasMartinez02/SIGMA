using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Persistence;
using SIGMA.API.Services;
using SIGMA.API.Users.Queries.Lookup;
using SIGMA.API.WorkOrders.Commands.AddMechanic;
using SIGMA.API.WorkOrders.Commands.AddTask;
using SIGMA.API.WorkOrders.Commands.AssignTask;
using SIGMA.API.WorkOrders.Commands.UpdateTaskStatus;
using Xunit;

namespace SIGMA.Tests.Application;

public class WorkOrderAssignmentTests
{
    private readonly ApplicationDbContext _context = TestDb.Create();

    private async Task<User> AddUserAsync(string email, UserRole role, bool active = true)
    {
        var user = User.Create(email, "hash", "Nombre", email, role);
        if (!active) user.SetActive(false);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private async Task<(WorkOrder WorkOrder, WorkOrderTask Task)> SeedWithTaskAsync()
    {
        var (_, _, workOrder) = await TestDb.SeedWorkOrderAsync(_context, DateTime.UtcNow);
        var task = WorkOrderTask.Create(workOrder.Id, 1, "Cambio de aceite", "Drenar y reponer", 2m, false);
        _context.WorkOrderTasks.Add(task);
        await _context.SaveChangesAsync();
        return (workOrder, task);
    }

    private static ICurrentUserService UserWith(Guid id, params string[] permissions)
    {
        var current = Substitute.For<ICurrentUserService>();
        current.UserId.Returns(id);
        current.Permissions.Returns(permissions.ToList());
        return current;
    }

    [Fact]
    public async Task Lookup_devuelve_solo_usuarios_activos_de_los_roles_pedidos()
    {
        await AddUserAsync("mec1@sigma.aero", UserRole.Mecanico);
        await AddUserAsync("mec2@sigma.aero", UserRole.Mecanico, active: false);
        await AddUserAsync("jefe@sigma.aero", UserRole.JefeTaller);
        await AddUserAsync("panol@sigma.aero", UserRole.Panol);

        var result = await new GetUserLookupQueryHandler(_context)
            .Handle(new GetUserLookupQuery([UserRole.Mecanico, UserRole.JefeTaller]), default);

        result.Select(u => u.Role).Should().BeEquivalentTo([UserRole.Mecanico, UserRole.JefeTaller]);
    }

    [Fact]
    public async Task Solo_se_asignan_mecanicos_o_jefes_activos()
    {
        var (workOrder, _) = await SeedWithTaskAsync();
        var panol = await AddUserAsync("panol@sigma.aero", UserRole.Panol);
        var inactive = await AddUserAsync("mec@sigma.aero", UserRole.Mecanico, active: false);
        var handler = new AddMechanicCommandHandler(_context);

        (await handler.Handle(new AddMechanicCommand(workOrder.Id, panol.Id), default)).Succeeded.Should().BeFalse();
        (await handler.Handle(new AddMechanicCommand(workOrder.Id, inactive.Id), default)).Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task La_tarea_solo_se_asigna_a_un_mecanico_de_la_OT()
    {
        var (workOrder, task) = await SeedWithTaskAsync();
        var mechanic = await AddUserAsync("mec@sigma.aero", UserRole.Mecanico);
        var handler = new AssignTaskCommandHandler(_context);

        var before = await handler.Handle(new AssignTaskCommand(workOrder.Id, task.Id, mechanic.Id), default);
        await new AddMechanicCommandHandler(_context).Handle(new AddMechanicCommand(workOrder.Id, mechanic.Id), default);
        var after = await handler.Handle(new AssignTaskCommand(workOrder.Id, task.Id, mechanic.Id), default);

        before.Succeeded.Should().BeFalse();
        after.Succeeded.Should().BeTrue();
        task.AssignedToId.Should().Be(mechanic.Id);
    }

    [Fact]
    public async Task El_mecanico_asignado_completa_su_tarea_y_las_horas_suben_a_la_OT()
    {
        var (workOrder, task) = await SeedWithTaskAsync();
        var mechanic = await AddUserAsync("mec@sigma.aero", UserRole.Mecanico);
        var outsider = await AddUserAsync("otro@sigma.aero", UserRole.Mecanico);
        await new AddMechanicCommandHandler(_context).Handle(new AddMechanicCommand(workOrder.Id, mechanic.Id), default);

        var denied = await new UpdateTaskStatusCommandHandler(_context, UserWith(outsider.Id, "canViewDashboard"))
            .Handle(new UpdateTaskStatusCommand(workOrder.Id, task.Id, WorkOrderTaskStatus.EnProceso, null), default);
        var handler = new UpdateTaskStatusCommandHandler(_context, UserWith(mechanic.Id, "canViewDashboard"));
        await handler.Handle(new UpdateTaskStatusCommand(workOrder.Id, task.Id, WorkOrderTaskStatus.EnProceso, null), default);
        var completed = await handler.Handle(
            new UpdateTaskStatusCommand(workOrder.Id, task.Id, WorkOrderTaskStatus.Completada, "Ok", 3m), default);

        denied.Succeeded.Should().BeFalse();
        completed.Succeeded.Should().BeTrue();
        (await _context.WorkOrders.FindAsync(workOrder.Id))!.ActualHours.Should().Be(3m);
    }

    [Fact]
    public async Task Una_OT_cancelada_no_admite_tareas_nuevas()
    {
        var (workOrder, _) = await SeedWithTaskAsync();
        workOrder.TransitionTo(WorkOrderStatus.Cancelada, Guid.NewGuid(), "Test", "Gerente");
        await _context.SaveChangesAsync();

        var result = await new AddWorkOrderTaskCommandHandler(_context, null!)
            .Handle(new AddWorkOrderTaskCommand(workOrder.Id, "Nueva", "Detalle", 1m, false), default);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("no admite cambios");
    }

    [Fact]
    public async Task El_numero_de_OT_ordena_numericamente_y_no_se_repite()
    {
        var dateTime = Substitute.For<IDateTimeProvider>();
        dateTime.CurrentYear.Returns(2091);
        var (_, aircraft, _) = await TestDb.SeedWorkOrderAsync(_context, DateTime.UtcNow);
        foreach (var number in new[] { "OT-2091-9999", "OT-2091-10000" })
            _context.WorkOrders.Add(WorkOrder.Create(number, WorkOrderType.Servicio, WorkOrderPriority.Baja,
                aircraft.Id, aircraft.ClientId, "Servicio", 1m, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 0m, Guid.NewGuid()));
        await _context.SaveChangesAsync();
        var generator = new WorkOrderNumberGenerator(_context, dateTime);

        var first = await generator.GenerateAsync();
        var second = await generator.GenerateAsync();

        first.Should().Be("OT-2091-10001");
        second.Should().Be("OT-2091-10002");
    }
}
