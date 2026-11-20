using FluentAssertions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using Xunit;

namespace SIGMA.Tests.Domain;

public class WorkOrderTaskRulesTests
{
    private static WorkOrderTask NewTask(bool requiresInspection = false) =>
        WorkOrderTask.Create(Guid.NewGuid(), 1, "Cambio de aceite", "Drenar y reponer", 2m, requiresInspection);

    [Fact]
    public void No_se_completa_una_tarea_pendiente_sin_pasar_por_en_proceso()
    {
        var task = NewTask();

        task.Invoking(t => t.UpdateStatus(WorkOrderTaskStatus.Completada, null, 2m))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Completar_exige_horas_reales_y_las_guarda()
    {
        var task = NewTask();
        task.UpdateStatus(WorkOrderTaskStatus.EnProceso, null);

        task.Invoking(t => t.UpdateStatus(WorkOrderTaskStatus.Completada, null)).Should().Throw<DomainException>();
        task.UpdateStatus(WorkOrderTaskStatus.Completada, null, 2.5m);

        task.ActualHours.Should().Be(2.5m);
        task.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reabrir_limpia_la_fecha_de_cierre()
    {
        var task = NewTask();
        task.UpdateStatus(WorkOrderTaskStatus.EnProceso, null);
        task.UpdateStatus(WorkOrderTaskStatus.Completada, null, 1m);

        task.UpdateStatus(WorkOrderTaskStatus.EnProceso, "Falta torque");

        task.Status.Should().Be(WorkOrderTaskStatus.EnProceso);
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Una_tarea_inspeccionada_no_se_puede_reabrir()
    {
        var task = NewTask(requiresInspection: true);
        task.UpdateStatus(WorkOrderTaskStatus.EnProceso, null);
        task.UpdateStatus(WorkOrderTaskStatus.Completada, null, 1m);
        task.MarkInspected(Guid.NewGuid());

        task.Invoking(t => t.UpdateStatus(WorkOrderTaskStatus.EnProceso, null)).Should().Throw<DomainException>();
    }

    [Fact]
    public void La_OT_suma_las_horas_reales_de_sus_tareas_y_expone_sus_transiciones()
    {
        var workOrder = WorkOrder.Create("OT-2026-0001", WorkOrderType.Reparacion, WorkOrderPriority.Media,
            Guid.NewGuid(), Guid.NewGuid(), "Reparacion", 4m, DateTime.UtcNow, DateTime.UtcNow.AddDays(2), 100m, Guid.NewGuid());
        foreach (var hours in new[] { 1.5m, 2m })
        {
            var task = WorkOrderTask.Create(workOrder.Id, 1, "Tarea", "Detalle", 1m, false);
            task.UpdateStatus(WorkOrderTaskStatus.EnProceso, null);
            task.UpdateStatus(WorkOrderTaskStatus.Completada, null, hours);
            workOrder.Tasks.Add(task);
        }

        workOrder.RecalculateActualHours();

        workOrder.ActualHours.Should().Be(3.5m);
        workOrder.GetAllowedTransitions().Should().BeEquivalentTo([WorkOrderStatus.EnProceso, WorkOrderStatus.Cancelada]);
    }
}
