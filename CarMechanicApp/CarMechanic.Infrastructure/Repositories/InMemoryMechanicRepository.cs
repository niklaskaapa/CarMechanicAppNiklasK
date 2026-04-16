using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Repositories;

public class InMemoryMechanicRepository : IMechanicRepository
{
    private readonly List<Mechanic> _mechanics = new();

    public Mechanic? GetById(int id) => _mechanics.FirstOrDefault(m => m.Id == id);
    public List<Mechanic> GetAll() => _mechanics.ToList();
    // BUG_TARGET: GetBySpecialty
    public List<Mechanic> GetBySpecialty(MechanicSpecialty specialty) => _mechanics.Where(m => m.Specialty == specialty).ToList();
    public void Add(Mechanic mechanic) => _mechanics.Add(mechanic);

    public void Update(Mechanic mechanic)
    {
        var index = _mechanics.FindIndex(m => m.Id == mechanic.Id);
        if (index >= 0)
            _mechanics[index] = mechanic;
    }
}
