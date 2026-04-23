using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Application.Services;

public class VehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private int _nextId = 1;

    public VehicleService(IVehicleRepository vehicleRepository, ICustomerRepository customerRepository)
    {
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
    }

    // BUG_TARGET: RegisterVehicle
    public Vehicle RegisterVehicle(string licensePlate, string brand, string model, int year, int ownerId, VehicleType vehicleType, int mileage)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new ArgumentException("License plate cannot be empty.");
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be empty.");
        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new ArgumentException("Invalid year.");
        if (mileage < 0)
            throw new ArgumentException("Mileage cannot be negative.");

        var customer = _customerRepository.GetById(ownerId);
        if (customer == null)
            throw new InvalidOperationException("Owner not found.");

        var vehicle = new Vehicle
        {
            Id = _nextId++,
            LicensePlate = licensePlate.ToUpper(),
            Brand = brand,
            Model = model,
            Year = year,
            OwnerId = ownerId,
            VehicleType = vehicleType,
            Mileage = mileage
        };

        _vehicleRepository.Add(vehicle);
        return vehicle;
    }

    // BUG_TARGET: UpdateMileage
    public void UpdateMileage(int vehicleId, int newMileage)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found.");
        if (newMileage < vehicle.Mileage)
            throw new ArgumentException("New mileage cannot be less than current mileage.");

        vehicle.Mileage = newMileage;
        _vehicleRepository.Update(vehicle);
    }

    // MISSING_TARGET: GetVehiclesByCustomer
    public List<Vehicle> GetVehiclesByCustomer(int customerId)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found.");

        return _vehicleRepository.GetByOwnerId(customerId);
    }

    // BUG_TARGET: SearchVehicles
    public List<Vehicle> SearchVehicles(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Search keyword cannot be empty.");

        return _vehicleRepository.Search(keyword);
    }

    // MISSING_TARGET: GetVehiclesDueForService
    public List<Vehicle> GetVehiclesDueForService(int mileageThreshold)
    {
        if (mileageThreshold <= 0)
            throw new ArgumentException("Mileage threshold must be positive.");

        return _vehicleRepository.GetAll()
            .Where(v => v.Mileage >= mileageThreshold)
            .ToList();
    }

    // BUG_TARGET: GetVehiclesByType
    public List<Vehicle> GetVehiclesByType(VehicleType type)
    {
        return _vehicleRepository.GetByType(type);
    }

    // MISSING_TARGET: GetVehicleHistory
    public string GetVehicleHistory(int vehicleId)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found.");

        var customer = _customerRepository.GetById(vehicle.OwnerId);
        var ownerName = customer?.FullName ?? "Okänd";

        return $"{vehicle.Brand} {vehicle.Model} ({vehicle.Year}) | Reg: {vehicle.LicensePlate} | Ägare: {ownerName} | Mätarställning: {vehicle.Mileage} km";
    }

    // BUG_TARGET: CalculateAverageAge
    public double CalculateAverageAge()
    {
        var vehicles = _vehicleRepository.GetAll();
        if (vehicles.Count == 0)
            return 0;

        var currentYear = DateTime.Now.Year;
        return vehicles.Average(v => currentYear - v.Year);
    }

    // MISSING_TARGET: TransferOwnership
    public void TransferOwnership(int vehicleId, int newOwnerId)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found.");

        var newOwner = _customerRepository.GetById(newOwnerId);
        if (newOwner == null)
            throw new InvalidOperationException("New owner not found.");

        vehicle.OwnerId = newOwnerId;
        _vehicleRepository.Update(vehicle);
    }

    // BUG_TARGET: GetVehicleCount
    public int GetVehicleCount()
    {
        return _vehicleRepository.GetAll().Count;
    }

    public Vehicle? GetVehicleById(int id)
    {
        return _vehicleRepository.GetById(id);
    }
}
