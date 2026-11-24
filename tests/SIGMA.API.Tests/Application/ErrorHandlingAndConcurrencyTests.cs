using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using SIGMA.API.Middleware;
using SIGMA.API.Persistence;
using Xunit;

namespace SIGMA.Tests.Application;

public class ErrorHandlingAndConcurrencyTests
{
    private static async Task<(int Status, JsonElement Body)> RunAsync(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(_ => throw exception, NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var body = await JsonDocument.ParseAsync(context.Response.Body);
        return (context.Response.StatusCode, body.RootElement);
    }

    [Fact]
    public async Task Validacion_Devuelve_422_Con_Los_Mensajes()
    {
        var (status, body) = await RunAsync(new ValidationException([new ValidationFailure("Name", "Completá el nombre.")]));

        status.Should().Be(422);
        body.GetProperty("success").GetBoolean().Should().BeFalse();
        body.GetProperty("errors")[0].GetString().Should().Be("Completá el nombre.");
    }

    [Fact]
    public async Task Regla_De_Negocio_Devuelve_400()
    {
        var (status, body) = await RunAsync(new DomainException("La OT está cerrada."));

        status.Should().Be(400);
        body.GetProperty("errors")[0].GetString().Should().Be("La OT está cerrada.");
    }

    [Fact]
    public async Task Conflicto_De_Concurrencia_Devuelve_409()
    {
        var (status, body) = await RunAsync(new DbUpdateConcurrencyException("conflicto"));

        status.Should().Be(409);
        body.GetProperty("errors")[0].GetString().Should().Contain("Recargá");
    }

    [Fact]
    public async Task Error_Inesperado_Devuelve_500_Sin_Detalles_Internos()
    {
        var (status, body) = await RunAsync(new InvalidOperationException("connection string secreta"));

        status.Should().Be(500);
        body.GetRawText().Should().NotContain("secreta");
    }

    [Fact]
    public async Task Dos_Movimientos_Simultaneos_Sobre_El_Mismo_Item_No_Pisan_El_Stock()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var item = InventoryItem.Create("PN-1", "Filtro de aceite", InventoryCategory.Consumible,
            "Champion", "Estante A", 2m, "unidades", 100m);
        item.ApplyMovement(10m, MovementType.Entrada);
        await using (var seed = new ApplicationDbContext(options))
        {
            seed.InventoryItems.Add(item);
            await seed.SaveChangesAsync();
        }

        await using var first = new ApplicationDbContext(options);
        await using var second = new ApplicationDbContext(options);
        var fromFirst = await first.InventoryItems.SingleAsync();
        var fromSecond = await second.InventoryItems.SingleAsync();

        fromFirst.ApplyMovement(3m, MovementType.Salida);
        await first.SaveChangesAsync();

        fromSecond.ApplyMovement(4m, MovementType.Salida);
        var act = () => second.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }
}
