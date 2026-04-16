using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Interfaces;

public interface IPartRepository
{
    Part? GetById(int id);
    List<Part> GetAll();
    List<Part> GetByCategory(PartCategory category);
    List<Part> Search(string keyword);
    void Add(Part part);
    void Update(Part part);
}
