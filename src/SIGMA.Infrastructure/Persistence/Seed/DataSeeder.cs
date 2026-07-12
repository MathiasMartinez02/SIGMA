using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;

namespace SIGMA.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    private static readonly string PasswordHash = BCrypt.Net.BCrypt.HashPassword("sigma2026");

    public static async Task SeedAsync(DbContext context, ILogger logger)
    {
        try
        {
            await SeedUsersAsync(context);
            await SeedClientsAsync(context);
            await SeedAircraftAsync(context);
            await SeedInventoryAsync(context);
            await SeedWorkOrdersAsync(context);
            await context.SaveChangesAsync();
            logger.LogInformation("Seed data applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying seed data.");
            throw;
        }
    }

    private static async Task SeedUsersAsync(DbContext context)
    {
        if (await context.Set<User>().AnyAsync()) return;

        var users = new List<User>
        {
            User.Create("gerente@sigma.aero", PasswordHash, "Carlos", "Rodríguez", UserRole.Gerente),
            User.Create("inspector@sigma.aero", PasswordHash, "María", "González", UserRole.Inspector, "INS-ANAC-0042"),
            User.Create("mecanico1@sigma.aero", PasswordHash, "Jorge", "Méndez", UserRole.Mecanico, "MEC-ANAC-0187"),
            User.Create("jefetaller@sigma.aero", PasswordHash, "Roberto", "Vásquez", UserRole.JefeTaller, "JT-ANAC-0023"),
            User.Create("panol@sigma.aero", PasswordHash, "Ana", "Torres", UserRole.Panol),
            User.Create("otecnica@sigma.aero", PasswordHash, "Luis", "Fernández", UserRole.OficinaTecnica),
            User.Create("admin@sigma.aero", PasswordHash, "Sofía", "Martínez", UserRole.Administracion),
        };

        await context.Set<User>().AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    private static async Task SeedClientsAsync(DbContext context)
    {
        if (await context.Set<Client>().AnyAsync()) return;

        var clients = new List<Client>
        {
            Client.Create("Aeroclub del Litoral", "Aeroclub del Litoral", "30-71234567-2",
                "contacto@aeroclublitoral.org", "0341-4520000",
                "Av. Jorge Newbery 2850", "Rosario", "Santa Fe",
                "Diego Palermo", "0341-155-123456"),

            Client.Create("Transportes Aéreos del Sur", "Transportes Aéreos del Sur S.A.", "30-68901234-5",
                "operaciones@tassur.com.ar", "011-4480-9000",
                "Ruta 205 Km 37.5", "Ezeiza", "Buenos Aires",
                "Patricia Vidal", "011-15-4567-8901"),

            Client.Create("Ejecutivos del Norte", "Ejecutivos del Norte S.A.", "30-72345678-1",
                "info@ejecutivosnorte.com", "0381-4311500",
                "Aeropuerto Teniente Benjamín Matienzo", "Tucumán", "Tucumán",
                "Martín Sosa", "0381-155-987654"),

            Client.Create("Helicópteros Patagonia", "Helicópteros Patagonia S.R.L.", "30-69012345-8",
                "vuelos@helipat.com.ar", "0299-4483100",
                "Av. Olascoaga 1120", "Neuquén", "Neuquén",
                "Gabriel Flores", "0299-154-333222"),
        };

        await context.Set<Client>().AddRangeAsync(clients);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAircraftAsync(DbContext context)
    {
        if (await context.Set<Aircraft>().AnyAsync()) return;

        var clients = await context.Set<Client>().ToListAsync();
        var aeroclubId = clients.First(c => c.Name == "Aeroclub del Litoral").Id;
        var tasId = clients.First(c => c.Name == "Transportes Aéreos del Sur").Id;
        var ejecutivosId = clients.First(c => c.Name == "Ejecutivos del Norte").Id;
        var helipatId = clients.First(c => c.Name == "Helicópteros Patagonia").Id;

        var aircraft = new List<Aircraft>
        {
            Aircraft.Create("LV-RGT", "172S", "Cessna", 2008, AircraftCategory.Avion,
                "17282493", "Lycoming IO-360-L2A", "L-30425-51A",
                1847.3m, 2410m, new DateTime(2026, 3, 15), new DateTime(2026, 9, 15),
                1900m, new DateTime(2026, 12, 31), aeroclubId),

            Aircraft.Create("LV-MNP", "PA-28-181 Archer III", "Piper", 2003, AircraftCategory.Avion,
                "2843102", "Lycoming O-360-A4M", "L-19278-36A",
                3421.0m, 4820m, new DateTime(2025, 11, 20), new DateTime(2026, 5, 20),
                3500m, new DateTime(2026, 8, 15), tasId),

            Aircraft.Create("LV-BCD", "Bonanza G36", "Beechcraft", 2018, AircraftCategory.Avion,
                "E-3841", "Continental IO-550-B", "674903",
                890.5m, 1120m, new DateTime(2026, 2, 10), new DateTime(2026, 8, 10),
                950m, new DateTime(2027, 2, 28), ejecutivosId),

            Aircraft.Create("LV-XYZ", "R44 Raven II", "Robinson", 2015, AircraftCategory.Helicoptero,
                "14328", "Lycoming IO-540-AE1A5", "L-38521-61A",
                2156.2m, 0m, new DateTime(2026, 1, 5), new DateTime(2026, 7, 5),
                2200m, new DateTime(2026, 9, 30), helipatId),

            Aircraft.Create("LV-KRT", "SR22T", "Cirrus", 2020, AircraftCategory.Avion,
                "0854", "Continental TSIO-550-K", "836292",
                445.8m, 610m, new DateTime(2025, 8, 20), new DateTime(2026, 2, 20),
                500m, new DateTime(2026, 4, 15), aeroclubId),
        };

        aircraft[1].UpdateStatus(AircraftStatus.EnMantenimiento);
        aircraft[2].UpdateStatus(AircraftStatus.EnInspeccion);
        aircraft[4].UpdateStatus(AircraftStatus.FueraDeServicio);

        await context.Set<Aircraft>().AddRangeAsync(aircraft);
        await context.SaveChangesAsync();
    }

    private static async Task SeedInventoryAsync(DbContext context)
    {
        if (await context.Set<InventoryItem>().AnyAsync()) return;

        var items = new List<InventoryItem>
        {
            InventoryItem.Create("LYC-OIL-W80-1Q", "Aceite Lycoming W80 1 Quart",
                InventoryCategory.Consumible, "Phillips 66", "Estante A-1",
                10, "litros", 850m),

            InventoryItem.Create("CMC-SPC-A10", "Bujía Champion REM37BY",
                InventoryCategory.RepuestoMotor, "Champion", "Cajón M-3",
                4, "unidades", 2200m, true, null, "[\"REM37BY\", \"AC-MA4K\"]"),

            InventoryItem.Create("MIL-HYD-5606", "Aceite Hidráulico MIL-H-5606",
                InventoryCategory.Consumible, "Aeroshell", "Estante B-2",
                5, "litros", 1800m),

            InventoryItem.Create("BRK-ASY-C172-MAIN", "Conjunto de Freno Principal C172",
                InventoryCategory.RepuestoCelula, "Cleveland", "Estante C-4",
                1, "set", 45000m, true, null),

            InventoryItem.Create("WHL-SKY-R44-TAIL", "Rueda de Cola R44",
                InventoryCategory.RepuestoCelula, "Robinson", "Estante D-1",
                2, "unidades", 18500m, true, null),
        };

        items[0].ApplyMovement(50, Domain.Enums.MovementType.Entrada);
        items[1].ApplyMovement(20, Domain.Enums.MovementType.Entrada);
        items[2].ApplyMovement(30, Domain.Enums.MovementType.Entrada);
        items[3].ApplyMovement(3, Domain.Enums.MovementType.Entrada);
        items[4].ApplyMovement(5, Domain.Enums.MovementType.Entrada);

        await context.Set<InventoryItem>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    private static async Task SeedWorkOrdersAsync(DbContext context)
    {
        if (await context.Set<WorkOrder>().AnyAsync()) return;

        var users = await context.Set<User>().ToListAsync();
        var aircraft = await context.Set<Aircraft>().ToListAsync();
        var clients = await context.Set<Client>().ToListAsync();

        var gerente = users.First(u => u.Role == UserRole.Gerente);
        var inspector = users.First(u => u.Role == UserRole.Inspector);
        var jefeTaller = users.First(u => u.Role == UserRole.JefeTaller);

        var lvRgt = aircraft.First(a => a.Registration == "LV-RGT");
        var lvMnp = aircraft.First(a => a.Registration == "LV-MNP");
        var lvBcd = aircraft.First(a => a.Registration == "LV-BCD");
        var lvXyz = aircraft.First(a => a.Registration == "LV-XYZ");
        var lvKrt = aircraft.First(a => a.Registration == "LV-KRT");

        var aeroclub = clients.First(c => c.Name == "Aeroclub del Litoral");
        var tas = clients.First(c => c.Name == "Transportes Aéreos del Sur");
        var ejecutivos = clients.First(c => c.Name == "Ejecutivos del Norte");
        var helipat = clients.First(c => c.Name == "Helicópteros Patagonia");

        var workOrders = new[]
        {
            WorkOrder.Create("OT-2026-0001", WorkOrderType.Inspeccion100h, WorkOrderPriority.Alta,
                lvRgt.Id, aeroclub.Id, "Inspección de 100 horas reglamentaria. Revisión completa de motor, celula y sistemas.",
                25m, new DateTime(2026, 4, 15), lvRgt.TotalFlightHours, gerente.Id),

            WorkOrder.Create("OT-2026-0002", WorkOrderType.Reparacion, WorkOrderPriority.Critica,
                lvMnp.Id, tas.Id, "Reparación de tren de aterrizaje principal. Daño por aterrizaje forzoso.",
                40m, new DateTime(2026, 4, 30), lvMnp.TotalFlightHours, gerente.Id),

            WorkOrder.Create("OT-2026-0003", WorkOrderType.InspeccionAnual, WorkOrderPriority.Media,
                lvBcd.Id, ejecutivos.Id, "Inspección anual obligatoria según RAAC Parte 43.",
                30m, new DateTime(2026, 5, 10), lvBcd.TotalFlightHours, gerente.Id),

            WorkOrder.Create("OT-2026-0004", WorkOrderType.Inspeccion50h, WorkOrderPriority.Baja,
                lvXyz.Id, helipat.Id, "Inspección de 50 horas del rotor principal y cola.",
                15m, new DateTime(2026, 5, 20), lvXyz.TotalFlightHours, gerente.Id),

            WorkOrder.Create("OT-2026-0005", WorkOrderType.AdCumplimiento, WorkOrderPriority.Alta,
                lvKrt.Id, aeroclub.Id, "Cumplimiento de Directiva de Aeronavegabilidad 2026-08-01. Inspección de tanques de combustible.",
                20m, new DateTime(2026, 6, 1), lvKrt.TotalFlightHours, gerente.Id),
        };

        workOrders[0].TransitionTo(WorkOrderStatus.EnProceso, gerente.Id, gerente.FullName, "Gerente");
        workOrders[1].TransitionTo(WorkOrderStatus.EnProceso, jefeTaller.Id, jefeTaller.FullName, "JefeTaller");
        workOrders[2].TransitionTo(WorkOrderStatus.EnProceso, gerente.Id, gerente.FullName, "Gerente");

        await context.Set<WorkOrder>().AddRangeAsync(workOrders);
        await context.SaveChangesAsync();

        var tasks1 = new[]
        {
            WorkOrderTask.Create(workOrders[0].Id, 1, "Revisión de motor", "Inspección visual y funcional del motor", 8m, true),
            WorkOrderTask.Create(workOrders[0].Id, 2, "Revisión de hélice", "Inspección de hélice y regulador", 3m, true),
            WorkOrderTask.Create(workOrders[0].Id, 3, "Revisión de controles", "Inspección del sistema de control de vuelo", 5m, false),
            WorkOrderTask.Create(workOrders[0].Id, 4, "Revisión de instrumentos", "Calibración y verificación de instrumentos", 4m, false),
        };

        var tasks2 = new[]
        {
            WorkOrderTask.Create(workOrders[1].Id, 1, "Desmontaje del tren", "Desmontaje completo del tren principal", 6m, false),
            WorkOrderTask.Create(workOrders[1].Id, 2, "Evaluación de daños", "Evaluación estructural de componentes", 8m, true),
            WorkOrderTask.Create(workOrders[1].Id, 3, "Reemplazo de componentes", "Reemplazo de piezas dañadas", 16m, false),
            WorkOrderTask.Create(workOrders[1].Id, 4, "Prueba funcional", "Prueba del tren antes de vuelo", 4m, true),
        };

        await context.Set<WorkOrderTask>().AddRangeAsync(tasks1);
        await context.Set<WorkOrderTask>().AddRangeAsync(tasks2);

        var timelines = new[]
        {
            WorkOrderTimeline.Create(workOrders[0].Id, "Creación", "Orden OT-2026-0001 creada",
                gerente.Id, gerente.FullName, "Gerente"),
            WorkOrderTimeline.Create(workOrders[1].Id, "Creación", "Orden OT-2026-0002 creada",
                gerente.Id, gerente.FullName, "Gerente"),
            WorkOrderTimeline.Create(workOrders[2].Id, "Creación", "Orden OT-2026-0003 creada",
                gerente.Id, gerente.FullName, "Gerente"),
        };

        await context.Set<WorkOrderTimeline>().AddRangeAsync(timelines);
        await context.SaveChangesAsync();
    }
}
