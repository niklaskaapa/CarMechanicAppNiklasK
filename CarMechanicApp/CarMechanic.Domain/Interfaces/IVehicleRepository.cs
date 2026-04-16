using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Interfaces;

public interface IVehicleRepository
{
    Vehicle? GetById(int id);
    List<Vehicle> GetAll();
    List<Vehicle> GetByOwnerId(int ownerId);
    List<Vehicle> GetByType(VehicleType type);
    List<Vehicle> Search(string keyword);
    void Add(Vehicle vehicle);
    void Update(Vehicle vehicle);
}
