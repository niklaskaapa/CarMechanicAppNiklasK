using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Repositories;

public class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _vehicles = new();

    public Vehicle? GetById(int id) => _vehicles.FirstOrDefault(v => v.Id == id);
    public List<Vehicle> GetAll() => _vehicles.ToList();
    // BUG_TARGET: GetByOwnerId
    public List<Vehicle> GetByOwnerId(int ownerId) => _vehicles.Where(v => v.OwnerId == ownerId).ToList();
    public List<Vehicle> GetByType(VehicleType type) => _vehicles.Where(v => v.VehicleType == type).ToList();
    public List<Vehicle> Search(string keyword) => _vehicles.Where(v =>
        v.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        v.Model.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        v.LicensePlate.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
    public void Add(Vehicle vehicle) => _vehicles.Add(vehicle);

    public void Update(Vehicle vehicle)
    {
        var index = _vehicles.FindIndex(v => v.Id == vehicle.Id);
        if (index >= 0)
            _vehicles[index] = vehicle;
    }
}
