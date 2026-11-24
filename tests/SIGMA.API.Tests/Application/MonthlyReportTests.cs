using FluentAssertions;
using SIGMA.API.Reports.Queries.WorkOrders;
using Xunit;

namespace SIGMA.Tests.Application;

public class MonthlyReportTests
{
    [Fact]
    public async Task El_informe_mensual_toma_las_OT_ingresadas_en_el_mes_incluido_el_ultimo_dia()
    {
        using var context = TestDb.Create();
        await TestDb.SeedWorkOrderAsync(context, new DateTime(2026, 10, 31));

        var handler = new GetWorkOrdersReportQueryHandler(context);
        var october = await handler.Handle(
            new GetWorkOrdersReportQuery(new DateTime(2026, 10, 1), new DateTime(2026, 10, 31), null, null, null), default);
        var november = await handler.Handle(
            new GetWorkOrdersReportQuery(new DateTime(2026, 11, 1), new DateTime(2026, 11, 30), null, null, null), default);

        october.TotalCount.Should().Be(1);
        november.TotalCount.Should().Be(0);
    }
}
