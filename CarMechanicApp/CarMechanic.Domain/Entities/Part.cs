using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Entities;

public class Part
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public PartCategory Category { get; set; }
}
