using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Repositories;

public class InMemoryPartRepository : IPartRepository
{
    private readonly List<Part> _parts = new();

    public Part? GetById(int id) => _parts.FirstOrDefault(p => p.Id == id);
    public List<Part> GetAll() => _parts.ToList();
    public List<Part> GetByCategory(PartCategory category) => _parts.Where(p => p.Category == category).ToList();
    // BUG_TARGET: Search
    public List<Part> Search(string keyword) => _parts.Where(p =>
        p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) &&
        p.PartNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
    public void Add(Part part) => _parts.Add(part);

    public void Update(Part part)
    {
        var index = _parts.FindIndex(p => p.Id == part.Id);
        if (index >= 0)
            _parts[index] = part;
    }
}
