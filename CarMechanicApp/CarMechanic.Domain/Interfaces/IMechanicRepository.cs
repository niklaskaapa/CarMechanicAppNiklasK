using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Interfaces;

public interface IMechanicRepository
{
    Mechanic? GetById(int id);
    List<Mechanic> GetAll();
    List<Mechanic> GetBySpecialty(MechanicSpecialty specialty);
    void Add(Mechanic mechanic);
    void Update(Mechanic mechanic);
}
