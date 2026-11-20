using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Aircraft.Commands.AddFlightRecord;
using SIGMA.API.Aircraft.Commands.UpdateStatus;
using SIGMA.API.Aircraft.Queries.GetById;
using SIGMA.API.Appointments.Commands.Update;
using SIGMA.API.Common.Mappings;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Inventory.Commands.Create;
using SIGMA.API.Persistence;
using Xunit;

namespace SIGMA.Tests.Application;

public class ModelGapsTests
{
    private readonly ApplicationDbContext _context = TestDb.Create();

    [Fact]
    public async Task El_detalle_de_aeronave_trae_los_vuelos_y_la_carga_suma_horas()
    {
        var (_, aircraft, _) = await TestDb.SeedWorkOrderAsync(_context, DateTime.Today);
        var addFlight = new AddFlightRecordCommandHandler(_context);
        await addFlight.Handle(new AddFlightRecordCommand(aircraft.Id, DateTime.Today.AddDays(-2), 1.5m, 2, "Piloto A", "SACO", "SAAR", null), default);
        await addFlight.Handle(new AddFlightRecordCommand(aircraft.Id, DateTime.Today.AddDays(-1), 2m, 1, "Piloto B", "SAAR", "SACO", null), default);

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AircraftMappingProfile>()).CreateMapper();
        var result = await new GetAircraftByIdQueryHandler(_context, mapper).Handle(new GetAircraftByIdQuery(aircraft.Id), default);

        result.Data!.FlightHistory.Should().HaveCount(2);
        result.Data.FlightHistory[0].Pilot.Should().Be("Piloto B");
        result.Data.TotalFlightHours.Should().Be(1503.5m);
        result.Data.TotalLandings.Should().Be(2003m);
    }

    [Fact]
    public async Task Estado_manual_de_aeronave_solo_fuera_de_servicio_o_vuelta_a_servicio()
    {
        var (_, aircraft, workOrder) = await TestDb.SeedWorkOrderAsync(_context, DateTime.Today);
        var handler = new UpdateAircraftStatusCommandHandler(_context);

        (await handler.Handle(new UpdateAircraftStatusCommand(aircraft.Id, AircraftStatus.EnMantenimiento), default))
            .Succeeded.Should().BeFalse();

        (await handler.Handle(new UpdateAircraftStatusCommand(aircraft.Id, AircraftStatus.FueraDeServicio), default))
            .Succeeded.Should().BeTrue();
        aircraft.Status.Should().Be(AircraftStatus.FueraDeServicio);

        workOrder.TransitionTo(WorkOrderStatus.EnProceso, Guid.NewGuid(), "Test", "Gerente");
        await _context.SaveChangesAsync();
        await handler.Handle(new UpdateAircraftStatusCommand(aircraft.Id, AircraftStatus.Operativa), default);

        aircraft.Status.Should().Be(AircraftStatus.EnMantenimiento);
    }

    [Fact]
    public async Task Editar_turno_asocia_la_aeronave_del_cliente_y_no_la_de_otro()
    {
        var (client, aircraft, _) = await TestDb.SeedWorkOrderAsync(_context, DateTime.Today);
        var otherClient = Client.Create("Otro", "Otro SA", "30-99999999-9", "otro@x.com", "351", "Calle 1", "Cordoba", "Cordoba");
        var otherAircraft = Aircraft.Create("LV-ZZZ", "PA-28", "Piper", 2010, AircraftCategory.Avion, "S", "O-320", "E",
            100m, 100m, null, null, 200m, DateTime.Today.AddYears(1), otherClient.Id);
        var appointment = Appointment.Create(client.Id, null, "LV-ABC", WorkOrderType.InspeccionAnual, DateTime.Now.AddDays(3));
        _context.AddRange(otherClient, otherAircraft, appointment);
        await _context.SaveChangesAsync();
        var handler = new UpdateAppointmentCommandHandler(_context);
        var when = DateTime.Now.AddDays(5);

        var wrong = await handler.Handle(new UpdateAppointmentCommand(appointment.Id, otherAircraft.Id, null, WorkOrderType.InspeccionAnual, when, null), default);
        var right = await handler.Handle(new UpdateAppointmentCommand(appointment.Id, aircraft.Id, null, WorkOrderType.InspeccionAnual, when, "Reprogramado"), default);

        wrong.Succeeded.Should().BeFalse();
        right.Succeeded.Should().BeTrue();
        appointment.AircraftId.Should().Be(aircraft.Id);
        appointment.AircraftRegistrationHint.Should().BeNull();
        appointment.ScheduledDate.Should().Be(when);
    }

    [Fact]
    public void Un_turno_cancelado_no_se_puede_editar()
    {
        var appointment = Appointment.Create(Guid.NewGuid(), null, "LV-ABC", WorkOrderType.Servicio, DateTime.Now.AddDays(1));
        appointment.Cancel();

        appointment.Invoking(a => a.Update(null, "LV-ABC", WorkOrderType.Servicio, DateTime.Now.AddDays(2), null))
            .Should().Throw<SIGMA.API.Exceptions.DomainException>();
    }

    [Fact]
    public async Task El_alta_de_item_guarda_el_certificado_y_lo_exige_si_requiere_certificacion()
    {
        var command = new CreateInventoryItemCommand("PN-CERT", "Magneto Slick 4370", InventoryCategory.RepuestoMotor,
            "Champion", "A1", 1, "u", 1000m, CertificationRequired: true, CertificateNumber: "8130-3 Nro 4521");

        var result = await new CreateInventoryItemCommandHandler(_context).Handle(command, default);
        var withoutCertificate = new CreateInventoryItemCommandValidator().Validate(command with { CertificateNumber = null });

        result.Succeeded.Should().BeTrue();
        (await _context.InventoryItems.SingleAsync()).CertificateNumber.Should().Be("8130-3 Nro 4521");
        withoutCertificate.IsValid.Should().BeFalse();
    }
}
