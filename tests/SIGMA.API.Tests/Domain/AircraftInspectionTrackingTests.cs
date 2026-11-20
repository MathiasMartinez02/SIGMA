using FluentAssertions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using Xunit;

namespace SIGMA.Tests.Domain;

public class AircraftInspectionTrackingTests
{
    private static Aircraft NewAircraft() =>
        Aircraft.Create("LV-ABC", "172S", "Cessna", 2015, AircraftCategory.Avion, "SN-1", "IO-360", "ESN-1",
            1500m, 2000m, null, null, 1550m, DateTime.UtcNow.AddYears(1), Guid.NewGuid());

    [Fact]
    public void Inspeccion_de_100h_actualiza_ultima_inspeccion_y_proximas_horas()
    {
        var aircraft = NewAircraft();
        var completedAt = new DateTime(2026, 11, 20);

        aircraft.RegisterCompletedInspection(WorkOrderType.Inspeccion100h, completedAt);

        aircraft.LastInspectionDate.Should().Be(completedAt);
        aircraft.NextInspectionHours.Should().Be(1600m);
    }

    [Fact]
    public void Inspeccion_anual_programa_la_proxima_a_un_anio()
    {
        var aircraft = NewAircraft();
        var completedAt = new DateTime(2026, 11, 20);

        aircraft.RegisterCompletedInspection(WorkOrderType.InspeccionAnual, completedAt);

        aircraft.NextInspectionDue.Should().Be(completedAt.AddYears(1));
    }

    [Fact]
    public void Una_reparacion_no_cambia_las_fechas_de_inspeccion()
    {
        var aircraft = NewAircraft();

        aircraft.RegisterCompletedInspection(WorkOrderType.Reparacion, DateTime.UtcNow);

        aircraft.LastInspectionDate.Should().BeNull();
        aircraft.NextInspectionHours.Should().Be(1550m);
    }

    [Theory]
    [InlineData(true, false, AircraftStatus.EnInspeccion)]
    [InlineData(false, true, AircraftStatus.EnMantenimiento)]
    [InlineData(false, false, AircraftStatus.Operativa)]
    public void Estado_segun_las_OT_activas(bool inInspection, bool inProgress, AircraftStatus expected)
    {
        var aircraft = NewAircraft();

        aircraft.SyncMaintenanceStatus(inInspection, inProgress);

        aircraft.Status.Should().Be(expected);
    }

    [Fact]
    public void Fuera_de_servicio_no_se_pisa()
    {
        var aircraft = NewAircraft();
        aircraft.UpdateStatus(AircraftStatus.FueraDeServicio);

        aircraft.SyncMaintenanceStatus(false, false);

        aircraft.Status.Should().Be(AircraftStatus.FueraDeServicio);
    }
}
