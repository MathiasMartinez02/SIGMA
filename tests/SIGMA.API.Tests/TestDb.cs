using Microsoft.EntityFrameworkCore;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Persistence;

namespace SIGMA.Tests;

public static class TestDb
{
    // Contexto InMemory aislado por test
    public static ApplicationDbContext Create() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    // Cliente, aeronave y OT minimos para queries de OT
    public static async Task<(Client Client, Aircraft Aircraft, WorkOrder WorkOrder)> SeedWorkOrderAsync(
        ApplicationDbContext context, DateTime intakeDate)
    {
        var client = Client.Create("Aeroclub Test", "Aeroclub Test SA", "30-12345678-9",
            "test@aeroclub.com", "3515555555", "Av. Siempreviva 742", "Cordoba", "Cordoba");
        var aircraft = Aircraft.Create("LV-ABC", "172S", "Cessna", 2015, AircraftCategory.Avion,
            "SN-1", "IO-360", "ESN-1", 1500m, 2000m, null, null, 1600m, DateTime.UtcNow.AddYears(1), client.Id);
        var workOrder = WorkOrder.Create("OT-2026-0001", WorkOrderType.Inspeccion100h, WorkOrderPriority.Media,
            aircraft.Id, client.Id, "Inspeccion de 100 horas", 8m, intakeDate, intakeDate.AddDays(5),
            aircraft.TotalFlightHours, Guid.NewGuid());

        context.Clients.Add(client);
        context.Aircraft.Add(aircraft);
        context.WorkOrders.Add(workOrder);
        await context.SaveChangesAsync();
        return (client, aircraft, workOrder);
    }
}
