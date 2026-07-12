using FluentAssertions;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;
using Xunit;

namespace SIGMA.Domain.Tests;

public class WorkOrderStatusTransitionTests
{
    private static WorkOrder CreateWorkOrder(WorkOrderStatus status = WorkOrderStatus.Pendiente)
    {
        var wo = WorkOrder.Create(
            "OT-2024-0001", WorkOrderType.Inspeccion100h, WorkOrderPriority.Media,
            Guid.NewGuid(), Guid.NewGuid(), "Test work order",
            10m, DateTime.UtcNow.AddDays(30), 100m, Guid.NewGuid());

        if (status == WorkOrderStatus.EnProceso)
            wo.TransitionTo(WorkOrderStatus.EnProceso, Guid.NewGuid(), "Test User", "Gerente");

        return wo;
    }

    [Fact]
    public void WorkOrder_NewState_Is_Pendiente()
    {
        var wo = CreateWorkOrder();
        wo.Status.Should().Be(WorkOrderStatus.Pendiente);
    }

    [Fact]
    public void WorkOrder_Pendiente_Can_Transition_To_EnProceso()
    {
        var wo = CreateWorkOrder();
        wo.TransitionTo(WorkOrderStatus.EnProceso, Guid.NewGuid(), "Test User", "Gerente");
        wo.Status.Should().Be(WorkOrderStatus.EnProceso);
    }

    [Fact]
    public void WorkOrder_Pendiente_Can_Transition_To_Cancelada()
    {
        var wo = CreateWorkOrder();
        wo.TransitionTo(WorkOrderStatus.Cancelada, Guid.NewGuid(), "Test User", "Gerente");
        wo.Status.Should().Be(WorkOrderStatus.Cancelada);
    }

    [Fact]
    public void WorkOrder_Pendiente_Cannot_Transition_To_Finalizada()
    {
        var wo = CreateWorkOrder();
        var act = () => wo.TransitionTo(WorkOrderStatus.Finalizada, Guid.NewGuid(), "Test User", "Gerente");
        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Fact]
    public void WorkOrder_Cancelada_Cannot_Transition_To_Any()
    {
        var wo = CreateWorkOrder();
        wo.TransitionTo(WorkOrderStatus.Cancelada, Guid.NewGuid(), "Test User", "Gerente");

        var act = () => wo.TransitionTo(WorkOrderStatus.Pendiente, Guid.NewGuid(), "Test User", "Gerente");
        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Fact]
    public void WorkOrder_EnProceso_Records_StartDate()
    {
        var wo = CreateWorkOrder();
        wo.TransitionTo(WorkOrderStatus.EnProceso, Guid.NewGuid(), "Test User", "Gerente");
        wo.StartDate.Should().NotBeNull();
        wo.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void WorkOrder_Transition_Adds_Timeline_Event()
    {
        var wo = CreateWorkOrder();
        wo.TransitionTo(WorkOrderStatus.EnProceso, Guid.NewGuid(), "Test User", "Gerente");
        wo.Timeline.Should().HaveCount(1);
        wo.Timeline.First().Event.Should().Contain("Estado");
    }

    [Fact]
    public void WorkOrder_EnProceso_Cannot_Transition_To_Finalizada_Directly()
    {
        var wo = CreateWorkOrder(WorkOrderStatus.EnProceso);
        var act = () => wo.TransitionTo(WorkOrderStatus.Finalizada, Guid.NewGuid(), "Test User", "Gerente");
        act.Should().Throw<InvalidStatusTransitionException>();
    }
}
