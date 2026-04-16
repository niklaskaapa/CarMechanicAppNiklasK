using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int OwnerId { get; set; }
    public VehicleType VehicleType { get; set; }
    public int Mileage { get; set; }
}
