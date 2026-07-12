using FluentAssertions;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;
using Xunit;

namespace SIGMA.Domain.Tests;

public class InventoryItemTests
{
    private static InventoryItem CreateItem(decimal currentStock = 10, decimal minimumStock = 5)
    {
        var item = InventoryItem.Create("TEST-001", "Test Part", InventoryCategory.Consumible,
            "Manufacturer", "Location", minimumStock, "units", 100m);
        if (currentStock > 0)
            item.ApplyMovement(currentStock, MovementType.Entrada);
        return item;
    }

    [Fact]
    public void Item_AboveMinimum_Status_Is_Disponible()
    {
        var item = CreateItem(currentStock: 10, minimumStock: 5);
        item.Status.Should().Be(InventoryStatus.Disponible);
    }

    [Fact]
    public void Item_AtMinimum_Status_Is_BajoStock()
    {
        var item = CreateItem(currentStock: 5, minimumStock: 5);
        item.Status.Should().Be(InventoryStatus.BajoStock);
    }

    [Fact]
    public void Item_BelowMinimum_Status_Is_BajoStock()
    {
        var item = CreateItem(currentStock: 3, minimumStock: 5);
        item.Status.Should().Be(InventoryStatus.BajoStock);
    }

    [Fact]
    public void Item_ZeroStock_Status_Is_SinStock()
    {
        var item = InventoryItem.Create("TEST-002", "Test Part", InventoryCategory.Consumible,
            "Manufacturer", "Location", 5, "units", 100m);
        item.Status.Should().Be(InventoryStatus.SinStock);
    }

    [Fact]
    public void Item_Salida_Reduces_Stock()
    {
        var item = CreateItem(10, 5);
        item.ApplyMovement(3, MovementType.Salida);
        item.CurrentStock.Should().Be(7);
    }

    [Fact]
    public void Item_Salida_Exceeding_Stock_Throws()
    {
        var item = CreateItem(5, 2);
        var act = () => item.ApplyMovement(10, MovementType.Salida);
        act.Should().Throw<DomainException>().WithMessage("*Stock insuficiente*");
    }

    [Fact]
    public void Item_Expired_Status_Is_Vencido()
    {
        var item = InventoryItem.Create("TEST-003", "Test Part", InventoryCategory.Consumible,
            "Manufacturer", "Location", 2, "units", 100m, false, DateTime.UtcNow.AddDays(-1));
        item.ApplyMovement(10, MovementType.Entrada);
        item.Status.Should().Be(InventoryStatus.Vencido);
    }

    [Fact]
    public void Item_Ajuste_Sets_Stock_To_Value()
    {
        var item = CreateItem(10, 5);
        item.ApplyMovement(25, MovementType.Ajuste);
        item.CurrentStock.Should().Be(25);
    }
}
