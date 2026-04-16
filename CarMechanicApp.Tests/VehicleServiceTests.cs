// Examination: Aron Åkesäter
// Generated: 2026-04-02
// Domain: CarMechanic

using CarMechanic.Application.Services;
using CarMechanic.Domain.Enums;
using CarMechanic.Infrastructure.Repositories;

namespace CarMechanic.Tests;

public class VehicleServiceTests
{
    private (VehicleService service, InMemoryCustomerRepository customerRepo) CreateService()
    {
        var vehicleRepo = new InMemoryVehicleRepository();
        var customerRepo = new InMemoryCustomerRepository();
        return (new VehicleService(vehicleRepo, customerRepo), customerRepo);
    }

    private (VehicleService service, int customerId) CreateServiceWithCustomer()
    {
        var vehicleRepo = new InMemoryVehicleRepository();
        var customerRepo = new InMemoryCustomerRepository();
        var service = new VehicleService(vehicleRepo, customerRepo);
        var customer = new CarMechanic.Domain.Entities.Customer
        {
            Id = 1,
            FirstName = "Anna",
            LastName = "Svensson",
            PhoneNumber = "070-1234567",
            Email = "anna@test.se"
        };
        customerRepo.Add(customer);
        return (service, customer.Id);
    }

    [Fact]
    public void RegisterVehicle_ValidData_ReturnsVehicle()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        var vehicle = service.RegisterVehicle("abc123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000);

        Assert.Equal("ABC123", vehicle.LicensePlate);
        Assert.Equal("Volvo", vehicle.Brand);
        Assert.Equal("V70", vehicle.Model);
        Assert.Equal(2020, vehicle.Year);
        Assert.Equal(50000, vehicle.Mileage);
    }

    [Fact]
    public void RegisterVehicle_EmptyLicensePlate_ThrowsArgumentException()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        Assert.Throws<ArgumentException>(() => service.RegisterVehicle("", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000));
    }

    [Fact]
    public void RegisterVehicle_InvalidOwner_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateService();
        Assert.Throws<InvalidOperationException>(() => service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, 999, VehicleType.Car, 50000));
    }

    [Fact]
    public void GetVehiclesByCustomer_ReturnsCorrectVehicles()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000);
        service.RegisterVehicle("DEF456", "Saab", "9-3", 2018, customerId, VehicleType.Car, 80000);

        var vehicles = service.GetVehiclesByCustomer(customerId);
        Assert.Equal(2, vehicles.Count);
    }

    [Fact]
    public void SearchVehicles_FindsByLicensePlate()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000);
        service.RegisterVehicle("DEF456", "Saab", "9-3", 2018, customerId, VehicleType.Car, 80000);

        var results = service.SearchVehicles("ABC");
        Assert.Single(results);
        Assert.Equal("ABC123", results[0].LicensePlate);
    }

    [Fact]
    public void GetVehiclesByType_ReturnsCorrectVehicles()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000);
        service.RegisterVehicle("DEF456", "Yamaha", "MT-07", 2022, customerId, VehicleType.Motorcycle, 10000);

        var cars = service.GetVehiclesByType(VehicleType.Car);
        Assert.Single(cars);
    }

    [Fact]
    public void GetVehicleCount_ReturnsCorrectCount()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000);
        service.RegisterVehicle("DEF456", "Saab", "9-3", 2018, customerId, VehicleType.Car, 80000);

        Assert.Equal(2, service.GetVehicleCount());
    }

    [Fact]
    public void GetVehiclesDueForService_HighMileage_ReturnsVehicle()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 90000);
        var due = service.GetVehiclesDueForService(80000);
        Assert.Single(due);
    }

    [Fact]
    public void GetVehiclesDueForService_LowMileage_ReturnsEmpty()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 20000);
        var due = service.GetVehiclesDueForService(80000);
        Assert.Empty(due);
    }

    [Fact]
    public void GetVehicleHistory_ValidVehicle_ReturnsFormattedString()
    {
        var (service, customerId) = CreateServiceWithCustomer();
        var vehicle = service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, customerId, VehicleType.Car, 50000);
        var history = service.GetVehicleHistory(vehicle.Id);
        Assert.Contains("Volvo", history);
        Assert.Contains("ABC123", history);
    }

    [Fact]
    public void GetVehicleHistory_InvalidVehicle_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateService();
        Assert.Throws<InvalidOperationException>(() => service.GetVehicleHistory(999));
    }

    [Fact]
    public void TransferOwnership_ValidData_UpdatesOwnerId()
    {
        var (service, customerRepo) = CreateService();
        customerRepo.Add(new CarMechanic.Domain.Entities.Customer
        {
            Id = 1, FirstName = "Anna", LastName = "Svensson",
            Email = "a@t.se", PhoneNumber = "070"
        });
        customerRepo.Add(new CarMechanic.Domain.Entities.Customer
        {
            Id = 2, FirstName = "Lars", LastName = "Nilsson",
            Email = "l@t.se", PhoneNumber = "071"
        });
        var vehicle = service.RegisterVehicle("ABC123", "Volvo", "V70", 2020, 1, VehicleType.Car, 50000);
        service.TransferOwnership(vehicle.Id, 2);
        var vehicles = service.GetVehiclesByCustomer(2);
        Assert.Single(vehicles);
    }
}
