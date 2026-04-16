// Examination: Aron Åkesäter
// Generated: 2026-04-02
// Domain: CarMechanic

using CarMechanic.Application.Services;
using CarMechanic.Domain.Enums;
using CarMechanic.Infrastructure.Repositories;

namespace CarMechanic.Tests;

public class PartServiceTests
{
    private PartService CreateService()
    {
        var partRepo = new InMemoryPartRepository();
        return new PartService(partRepo);
    }

    [Fact]
    public void AddPart_ValidData_ReturnsPart()
    {
        var service = CreateService();
        var part = service.AddPart("Oljefilter", "OF-123", 89.90m, 50, PartCategory.Engine);

        Assert.Equal("Oljefilter", part.Name);
        Assert.Equal("OF-123", part.PartNumber);
        Assert.Equal(89.90m, part.Price);
        Assert.Equal(50, part.StockQuantity);
    }

    [Fact]
    public void AddPart_EmptyName_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() => service.AddPart("", "OF-123", 89.90m, 50, PartCategory.Engine));
    }

    [Fact]
    public void AddPart_NegativePrice_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() => service.AddPart("Oljefilter", "OF-123", -10m, 50, PartCategory.Engine));
    }

    [Fact]
    public void RestockPart_IncreasesStock()
    {
        var service = CreateService();
        var part = service.AddPart("Oljefilter", "OF-123", 89.90m, 50, PartCategory.Engine);
        service.RestockPart(part.Id, 20);

        var updated = service.GetPartById(part.Id);
        Assert.Equal(70, updated!.StockQuantity);
    }

    [Fact]
    public void UsePart_DecreasesStock()
    {
        var service = CreateService();
        var part = service.AddPart("Oljefilter", "OF-123", 89.90m, 50, PartCategory.Engine);
        service.UsePart(part.Id, 10);

        var updated = service.GetPartById(part.Id);
        Assert.Equal(40, updated!.StockQuantity);
    }

    [Fact]
    public void GetLowStockParts_ReturnsBelowThreshold()
    {
        var service = CreateService();
        service.AddPart("Oljefilter", "OF-123", 89.90m, 5, PartCategory.Engine);
        service.AddPart("Bromsbelägg", "BB-456", 249m, 50, PartCategory.Brakes);
        service.AddPart("Luftfilter", "LF-789", 129m, 3, PartCategory.Engine);

        var lowStock = service.GetLowStockParts(10);
        Assert.Equal(2, lowStock.Count);
    }

    [Fact]
    public void GetTotalStockValue_ReturnsCorrectValue()
    {
        var service = CreateService();
        service.AddPart("Oljefilter", "OF-123", 100m, 10, PartCategory.Engine);

        var value = service.GetTotalStockValue();
        Assert.Equal(1000m, value);
    }

    [Fact]
    public void GetPartsByCategory_ReturnsCorrectParts()
    {
        var service = CreateService();
        service.AddPart("Oljefilter", "OF-123", 89.90m, 50, PartCategory.Engine);
        service.AddPart("Bromsbelägg", "BB-456", 249m, 30, PartCategory.Brakes);
        service.AddPart("Luftfilter", "LF-789", 129m, 20, PartCategory.Engine);

        var engineParts = service.GetPartsByCategory(PartCategory.Engine);
        Assert.Equal(2, engineParts.Count);
    }
}
