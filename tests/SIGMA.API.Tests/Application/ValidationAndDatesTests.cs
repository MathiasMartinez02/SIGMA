using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Aircraft.Commands.Create;
using SIGMA.API.Appointments.Commands.Create;
using SIGMA.API.Clients.Commands.Update;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Enums;
using SIGMA.API.Persistence;
using SIGMA.API.WorkOrders.Commands.Create;
using SIGMA.API.WorkOrders.Commands.Update;
using Xunit;

namespace SIGMA.Tests.Application;

public class ValidationAndDatesTests
{
    private static UpdateClientCommand Client(string taxId = "30-12345678-9", string phone = "3515555555") =>
        new(Guid.NewGuid(), "Aeroclub", "Aeroclub SA", taxId, "info@aeroclub.com", phone,
            "Av. Siempreviva 742", "Cordoba", "Cordoba", null, null);

    private static CreateAircraftCommand Aircraft(string registration, DateTime certificateExpiry) =>
        new(registration, "172S", "Cessna", 2015, AircraftCategory.Avion, "SN-1", "IO-360", "ESN-1",
            1500m, 2000m, null, null, 1600m, certificateExpiry, Guid.NewGuid());

    [Fact]
    public void Editar_cliente_valida_CUIT_y_largos_de_la_base()
    {
        var validator = new UpdateClientCommandValidator();

        validator.Validate(Client()).IsValid.Should().BeTrue();
        validator.Validate(Client(taxId: "30123456789")).IsValid.Should().BeFalse();
        validator.Validate(Client(phone: new string('1', 31))).Errors
            .Should().ContainSingle().Which.ErrorMessage.Should().Be("El teléfono no puede superar los 30 caracteres.");
    }

    [Fact]
    public void Mensaje_de_campo_requerido_sin_problemas_de_genero()
    {
        var errors = new UpdateClientCommandValidator().Validate(Client() with { Address = "" }).Errors;

        errors.Should().ContainSingle().Which.ErrorMessage.Should().Be("Completá la dirección.");
    }

    [Fact]
    public void Matricula_en_minusculas_es_valida_como_en_el_dominio()
    {
        new CreateAircraftCommandValidator().Validate(Aircraft("lv-abcd", DateTime.Today.AddYears(1)))
            .IsValid.Should().BeTrue();
    }

    [Fact]
    public void Certificado_que_vence_hoy_es_valido_y_uno_vencido_no()
    {
        var validator = new CreateAircraftCommandValidator();

        validator.Validate(Aircraft("LV-ABC", DateTime.Today)).IsValid.Should().BeTrue();
        validator.Validate(Aircraft("LV-ABC", DateTime.Today.AddDays(-1))).IsValid.Should().BeFalse();
    }

    [Fact]
    public void OT_con_fin_estimado_hoy_es_valida()
    {
        var command = new CreateWorkOrderCommand(WorkOrderType.Servicio, WorkOrderPriority.Media, Guid.NewGuid(),
            "Servicio de rutina", 2m, DateTime.Today, DateTime.Today);

        new CreateWorkOrderCommandValidator().Validate(command).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Editar_OT_rechaza_horas_negativas_y_descripcion_vacia()
    {
        var command = new UpdateWorkOrderCommand(Guid.NewGuid(), "", -1m, DateTime.Today, DateTime.Today, WorkOrderPriority.Alta);

        new UpdateWorkOrderCommandValidator().Validate(command).Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Turno_se_compara_con_la_hora_local()
    {
        var validator = new CreateAppointmentCommandValidator();
        CreateAppointmentCommand At(DateTime when) =>
            new(Guid.NewGuid(), null, "LV-XYZ", WorkOrderType.InspeccionAnual, when, null);

        validator.Validate(At(DateTime.Now.AddHours(1))).IsValid.Should().BeTrue();
        validator.Validate(At(DateTime.Now.AddMinutes(-5))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task La_paginacion_normaliza_pagina_y_tamaño()
    {
        using var context = TestDb.Create();
        await TestDb.SeedWorkOrderAsync(context, DateTime.Today);

        var result = await context.WorkOrders.ToPaginatedResultAsync(0, 0);
        var big = await context.WorkOrders.ToPaginatedResultAsync(1, 1000);

        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        big.PageSize.Should().Be(PaginationExtensions.MaxPageSize);
    }

    [Fact]
    public async Task Los_instantes_vuelven_en_UTC_y_las_fechas_de_negocio_sin_zona()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        Guid workOrderId;
        await using (var seed = new ApplicationDbContext(options))
            workOrderId = (await TestDb.SeedWorkOrderAsync(seed, new DateTime(2026, 11, 20))).WorkOrder.Id;

        await using var context = new ApplicationDbContext(options);
        var workOrder = await context.WorkOrders.AsNoTracking().FirstAsync(w => w.Id == workOrderId);

        workOrder.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        workOrder.IntakeDate.Kind.Should().Be(DateTimeKind.Unspecified);
    }
}
