using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Enums;
using SIGMA.API.Persistence;
using Xunit;

namespace SIGMA.Tests.Application;

public class ChildEntityTrackingTests
{
    [Fact]
    public async Task Hijo_agregado_por_navegacion_queda_como_Added()
    {
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(dbName).Options;

        Guid workOrderId;
        await using (var seedContext = new ApplicationDbContext(options))
        {
            var (_, _, workOrder) = await TestDb.SeedWorkOrderAsync(seedContext, DateTime.UtcNow);
            workOrderId = workOrder.Id;
        }

        await using var context = new ApplicationDbContext(options);
        var loaded = await context.WorkOrders
            .Include(w => w.Tasks)
            .Include(w => w.Timeline)
            .FirstAsync(w => w.Id == workOrderId);

        loaded.TransitionTo(WorkOrderStatus.EnProceso, Guid.NewGuid(), "Test", "Gerente");
        context.ChangeTracker.DetectChanges();

        var newEntry = loaded.Timeline.Single();
        context.Entry(newEntry).State.Should().Be(EntityState.Added);
    }
}
