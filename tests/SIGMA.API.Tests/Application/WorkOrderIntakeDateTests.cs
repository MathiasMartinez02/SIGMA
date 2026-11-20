using FluentAssertions;
using SIGMA.API.Aircraft.Queries.GetMaintenanceHistory;
using SIGMA.API.WorkOrders.Queries.GetAll;
using Xunit;

namespace SIGMA.Tests.Application;

public class WorkOrderIntakeDateTests
{
    private static readonly DateTime IntakeDate = new(2026, 11, 18);

    [Fact]
    public async Task Listado_de_OT_devuelve_la_fecha_de_ingreso()
    {
        using var context = TestDb.Create();
        await TestDb.SeedWorkOrderAsync(context, IntakeDate);

        var result = await new GetWorkOrdersQueryHandler(context, null!)
            .Handle(new GetWorkOrdersQuery(), CancellationToken.None);

        result.Data.Should().ContainSingle().Which.IntakeDate.Should().Be(IntakeDate);
    }

    [Fact]
    public async Task Historial_de_aeronave_devuelve_la_fecha_de_ingreso()
    {
        using var context = TestDb.Create();
        var (_, aircraft, _) = await TestDb.SeedWorkOrderAsync(context, IntakeDate);

        var result = await new GetMaintenanceHistoryQueryHandler(context, null!)
            .Handle(new GetMaintenanceHistoryQuery(aircraft.Id), CancellationToken.None);

        result.Data.Should().ContainSingle().Which.IntakeDate.Should().Be(IntakeDate);
    }
}
