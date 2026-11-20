using FluentAssertions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using SIGMA.API.Inventory.Commands.AddMovement;
using Xunit;

namespace SIGMA.Tests.Domain;

public class InventoryMovementRulesTests
{
    private static InventoryItem ItemWithStock(decimal stock)
    {
        var item = InventoryItem.Create("PN-1", "Filtro de aceite", InventoryCategory.Consumible,
            "Lycoming", "A1", 2, "u", 100m);
        item.ApplyMovement(stock, MovementType.Entrada);
        return item;
    }

    [Theory]
    [InlineData(MovementType.Salida)]
    [InlineData(MovementType.Entrada)]
    [InlineData(MovementType.Reserva)]
    [InlineData(MovementType.Devolucion)]
    public void Cantidad_negativa_o_cero_se_rechaza(MovementType type)
    {
        var item = ItemWithStock(10);

        item.Invoking(i => i.ApplyMovement(-5, type)).Should().Throw<DomainException>();
        item.Invoking(i => i.ApplyMovement(0, type)).Should().Throw<DomainException>();
        item.CurrentStock.Should().Be(10);
    }

    [Fact]
    public void Salida_que_excede_el_stock_no_modifica_la_entidad()
    {
        var item = ItemWithStock(5);

        item.Invoking(i => i.ApplyMovement(10, MovementType.Salida)).Should().Throw<DomainException>();

        item.CurrentStock.Should().Be(5);
    }

    [Fact]
    public void Ajuste_a_cero_esta_permitido()
    {
        var item = ItemWithStock(5);

        item.ApplyMovement(0, MovementType.Ajuste);

        item.CurrentStock.Should().Be(0);
        item.Status.Should().Be(InventoryStatus.SinStock);
    }

    [Theory]
    [InlineData(MovementType.Salida, -5, false)]
    [InlineData(MovementType.Salida, 0, false)]
    [InlineData(MovementType.Salida, 3, true)]
    [InlineData(MovementType.Ajuste, 0, true)]
    [InlineData(MovementType.Ajuste, -1, false)]
    public void Validator_controla_la_cantidad(MovementType type, decimal quantity, bool expectedValid)
    {
        var command = new AddInventoryMovementCommand(Guid.NewGuid(), type, quantity, null, "Uso en OT");

        new AddInventoryMovementCommandValidator().Validate(command).IsValid.Should().Be(expectedValid);
    }
}
