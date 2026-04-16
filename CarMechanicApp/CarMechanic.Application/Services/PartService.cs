using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Application.Services;

public class PartService
{
    private readonly IPartRepository _partRepository;
    private int _nextId = 1;

    public PartService(IPartRepository partRepository)
    {
        _partRepository = partRepository;
    }

    // BUG_TARGET: AddPart
    public Part AddPart(string name, string partNumber, decimal price, int stockQuantity, PartCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Part name cannot be empty.");
        if (string.IsNullOrWhiteSpace(partNumber))
            throw new ArgumentException("Part number cannot be empty.");
        if (price <= 0)
            throw new ArgumentException("Price must be positive.");
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        var part = new Part
        {
            Id = _nextId++,
            Name = name,
            PartNumber = partNumber,
            Price = price,
            StockQuantity = stockQuantity,
            Category = category
        };

        _partRepository.Add(part);
        return part;
    }

    // BUG_TARGET: RestockPart
    public void RestockPart(int partId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var part = _partRepository.GetById(partId);
        if (part == null)
            throw new InvalidOperationException("Part not found.");

        part.StockQuantity += quantity;
        _partRepository.Update(part);
    }

    // MISSING_TARGET: UsePart
    public void UsePart(int partId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var part = _partRepository.GetById(partId);
        if (part == null)
            throw new InvalidOperationException("Part not found.");
        if (part.StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        part.StockQuantity -= quantity;
        _partRepository.Update(part);
    }

    // BUG_TARGET: SearchParts
    public List<Part> SearchParts(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword cannot be empty.");

        return _partRepository.Search(keyword);
    }

    // MISSING_TARGET: GetLowStockParts
    public List<Part> GetLowStockParts(int threshold)
    {
        if (threshold < 0)
            throw new ArgumentException("Threshold cannot be negative.");

        return _partRepository.GetAll()
            .Where(p => p.StockQuantity <= threshold)
            .ToList();
    }

    // BUG_TARGET: CalculatePartsCost
    public decimal CalculatePartsCost(List<(int PartId, int Quantity)> parts)
    {
        if (parts == null || parts.Count == 0)
            throw new ArgumentException("Parts list cannot be empty.");

        decimal total = 0;
        foreach (var (partId, quantity) in parts)
        {
            var part = _partRepository.GetById(partId);
            if (part == null)
                throw new InvalidOperationException($"Part {partId} not found.");

            total += part.Price * quantity;
        }

        return Math.Round(total, 2);
    }

    // MISSING_TARGET: GetPartsByCategory
    public List<Part> GetPartsByCategory(PartCategory category)
    {
        throw new NotImplementedException();
    }

    // BUG_TARGET: GetTotalStockValue
    public decimal GetTotalStockValue()
    {
        return _partRepository.GetAll().Sum(p => p.Price * p.StockQuantity);
    }

    // MISSING_TARGET: GetPartCount
    public int GetPartCount()
    {
        return _partRepository.GetAll().Count;
    }

    public Part? GetPartById(int id)
    {
        return _partRepository.GetById(id);
    }
}
