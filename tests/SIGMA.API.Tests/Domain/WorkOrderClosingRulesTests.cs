using FluentAssertions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using Xunit;

namespace SIGMA.Tests.Domain;

public class WorkOrderClosingRulesTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static WorkOrder WorkOrderInInspection()
    {
        var workOrder = WorkOrder.Create("OT-2026-0001", WorkOrderType.Reparacion, WorkOrderPriority.Media,
            Guid.NewGuid(), Guid.NewGuid(), "Reparacion de tren", 4m, DateTime.UtcNow, DateTime.UtcNow.AddDays(3),
            1500m, UserId);
        workOrder.TransitionTo(WorkOrderStatus.EnProceso, UserId, "Test", "Gerente");
        workOrder.TransitionTo(WorkOrderStatus.EnInspeccion, UserId, "Test", "Gerente");
        return workOrder;
    }

    private static void Approve(Inspection inspection)
    {
        inspection.Start(UserId);
        foreach (var item in inspection.ChecklistSections.SelectMany(s => s.Items).ToList())
            inspection.CheckItem(item.Id, ChecklistItemStatus.Conforme, null, UserId);
        inspection.Complete();
        inspection.Approve(UserId);
    }

    [Fact]
    public void No_se_finaliza_sin_inspeccion_aprobada_y_el_estado_no_cambia()
    {
        var workOrder = WorkOrderInInspection();
        workOrder.OpenInspection(1500m);

        workOrder.Invoking(w => w.TransitionTo(WorkOrderStatus.Finalizada, UserId, "Test", "Gerente"))
            .Should().Throw<DomainException>().WithMessage("*inspección aprobada*");

        workOrder.Status.Should().Be(WorkOrderStatus.EnInspeccion);
        workOrder.CompletedDate.Should().BeNull();
    }

    [Fact]
    public void Se_finaliza_con_la_inspeccion_aprobada()
    {
        var workOrder = WorkOrderInInspection();
        var (inspection, _) = workOrder.OpenInspection(1500m);
        Approve(inspection);

        workOrder.TransitionTo(WorkOrderStatus.Finalizada, UserId, "Test", "Gerente");

        workOrder.Status.Should().Be(WorkOrderStatus.Finalizada);
    }

    [Fact]
    public void Reutiliza_la_inspeccion_abierta_y_crea_otra_solo_tras_un_rechazo()
    {
        var workOrder = WorkOrderInInspection();
        var (first, firstIsNew) = workOrder.OpenInspection(1500m);
        var (again, againIsNew) = workOrder.OpenInspection(1500m);

        firstIsNew.Should().BeTrue();
        againIsNew.Should().BeFalse();
        again.Should().BeSameAs(first);

        first.Start(UserId);
        first.Reject("Torque fuera de tolerancia", UserId);
        var (second, secondIsNew) = workOrder.OpenInspection(1500m);

        secondIsNew.Should().BeTrue();
        workOrder.Inspections.Should().HaveCount(2);
        second.Should().NotBeSameAs(first);
    }
}
