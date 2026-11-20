using FluentAssertions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using Xunit;

namespace SIGMA.Tests.Domain;

public class InspectionLifecycleTests
{
    private static readonly Guid InspectorId = Guid.NewGuid();

    private static Inspection NewInspection(WorkOrderType type = WorkOrderType.Inspeccion100h) =>
        Inspection.Create(Guid.NewGuid(), type.ToString(), Guid.NewGuid(), 1500m, DateTime.UtcNow);

    private static IEnumerable<ChecklistItem> Items(Inspection inspection) =>
        inspection.ChecklistSections.SelectMany(s => s.Items);

    private static void CheckAll(Inspection inspection, ChecklistItemStatus status = ChecklistItemStatus.Conforme)
    {
        foreach (var item in Items(inspection).ToList())
            inspection.CheckItem(item.Id, status, "ok", InspectorId);
    }

    [Fact]
    public void Inspeccion_de_100h_nace_con_el_checklist_completo_de_la_plantilla()
    {
        var inspection = NewInspection();

        inspection.ChecklistSections.Should().HaveCount(8);
        Items(inspection).Should().OnlyContain(i => i.Status == ChecklistItemStatus.Pendiente);
        inspection.ChecklistSections.Select(s => s.OrderIndex).Should().BeInAscendingOrder();
    }

    [Fact]
    public void Reparacion_usa_el_checklist_de_conformidad()
    {
        NewInspection(WorkOrderType.Reparacion).ChecklistSections.Should().ContainSingle()
            .Which.Title.Should().Be("Conformidad de los trabajos");
    }

    [Fact]
    public void No_se_puede_controlar_el_checklist_sin_iniciar()
    {
        var inspection = NewInspection();
        var item = Items(inspection).First();

        inspection.Invoking(i => i.CheckItem(item.Id, ChecklistItemStatus.Conforme, null, InspectorId))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Iniciar_asigna_el_inspector_y_no_se_puede_iniciar_dos_veces()
    {
        var inspection = NewInspection();

        inspection.Start(InspectorId);

        inspection.Status.Should().Be(InspectionStatus.EnProceso);
        inspection.InspectorId.Should().Be(InspectorId);
        inspection.Invoking(i => i.Start(InspectorId)).Should().Throw<DomainException>();
    }

    [Fact]
    public void No_conforme_exige_observacion()
    {
        var inspection = NewInspection();
        inspection.Start(InspectorId);
        var item = Items(inspection).First();

        inspection.Invoking(i => i.CheckItem(item.Id, ChecklistItemStatus.NoConforme, " ", InspectorId))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void No_se_completa_con_items_pendientes()
    {
        var inspection = NewInspection();
        inspection.Start(InspectorId);
        var first = Items(inspection).First();
        inspection.CheckItem(first.Id, ChecklistItemStatus.Conforme, null, InspectorId);

        inspection.Invoking(i => i.Complete()).Should().Throw<DomainException>().WithMessage("*sin controlar*");
    }

    [Fact]
    public void Ciclo_completo_hasta_aprobar()
    {
        var inspection = NewInspection();
        inspection.Start(InspectorId);
        CheckAll(inspection);

        inspection.Complete("Sin novedades");
        inspection.Approve(InspectorId);

        inspection.Status.Should().Be(InspectionStatus.Aprobada);
        inspection.CompletedAt.Should().NotBeNull();
        inspection.ApprovedById.Should().Be(InspectorId);
    }

    [Fact]
    public void No_se_aprueba_en_proceso_ni_con_no_conformidades()
    {
        var inspection = NewInspection(WorkOrderType.Reparacion);
        inspection.Start(InspectorId);
        inspection.Invoking(i => i.Approve(InspectorId)).Should().Throw<DomainException>();

        CheckAll(inspection, ChecklistItemStatus.NoConforme);
        inspection.Complete();

        inspection.Invoking(i => i.Approve(InspectorId)).Should().Throw<DomainException>().WithMessage("*No conformes*");
    }

    [Fact]
    public void No_se_puede_rechazar_una_inspeccion_aprobada()
    {
        var inspection = NewInspection(WorkOrderType.Reparacion);
        inspection.Start(InspectorId);
        CheckAll(inspection);
        inspection.Complete();
        inspection.Approve(InspectorId);

        inspection.Invoking(i => i.Reject("cambio de opinion", InspectorId)).Should().Throw<DomainException>();
        inspection.Status.Should().Be(InspectionStatus.Aprobada);
    }
}
