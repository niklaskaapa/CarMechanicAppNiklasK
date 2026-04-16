// Examination: Aron Åkesäter
// Generated: 2026-04-02
// Domain: CarMechanic

using CarMechanic.Application.Services;
using CarMechanic.Domain.Enums;
using CarMechanic.Infrastructure.Repositories;
using CarMechanic.Infrastructure.Factories;

namespace CarMechanic.Tests;

public class WorkOrderServiceTests
{
    private (WorkOrderService service, int vehicleId, int mechanicId) CreateServiceWithData()
    {
        var workOrderRepo = new InMemoryWorkOrderRepository();
        var vehicleRepo = new InMemoryVehicleRepository();
        var mechanicRepo = new InMemoryMechanicRepository();
        var factory = new WorkOrderFactory();

        var vehicle = new CarMechanic.Domain.Entities.Vehicle
        {
            Id = 1,
            LicensePlate = "ABC123",
            Brand = "Volvo",
            Model = "V70",
            Year = 2020,
            VehicleType = VehicleType.Car,
            OwnerId = 1,
            Mileage = 50000
        };
        vehicleRepo.Add(vehicle);

        var mechanic = new CarMechanic.Domain.Entities.Mechanic
        {
            Id = 1,
            FirstName = "Erik",
            LastName = "Johansson",
            Specialty = MechanicSpecialty.Engine,
            HourlyRate = 350m
        };
        mechanicRepo.Add(mechanic);

        var service = new WorkOrderService(workOrderRepo, vehicleRepo, mechanicRepo, factory);
        return (service, vehicle.Id, mechanic.Id);
    }

    [Fact]
    public void CreateWorkOrder_ValidData_ReturnsWorkOrder()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);

        Assert.Equal("Oljebyte", order.Description);
        Assert.Equal(WorkOrderStatus.Pending, order.Status);
    }

    [Fact]
    public void CreateWorkOrder_EmptyDescription_ThrowsArgumentException()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        Assert.Throws<ArgumentException>(() => service.CreateWorkOrder(vehicleId, mechanicId, "", 2m));
    }

    [Fact]
    public void CreateWorkOrder_InvalidVehicle_ThrowsInvalidOperationException()
    {
        var (service, _, mechanicId) = CreateServiceWithData();
        Assert.Throws<InvalidOperationException>(() => service.CreateWorkOrder(999, mechanicId, "Oljebyte", 2m));
    }

    [Fact]
    public void CompleteWorkOrder_SetsStatusToCompleted()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.StartWorkOrder(order.Id);
        service.CompleteWorkOrder(order.Id, 2m);

        var updated = service.GetWorkOrderById(order.Id);
        Assert.Equal(WorkOrderStatus.Completed, updated!.Status);
        Assert.Equal(2m, updated.ActualHours);
    }

    [Fact]
    public void StartWorkOrder_SetsStatusToInProgress()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.StartWorkOrder(order.Id);

        var updated = service.GetWorkOrderById(order.Id);
        Assert.Equal(WorkOrderStatus.InProgress, updated!.Status);
    }

    [Fact]
    public void CalculateEstimatedCost_ReturnsCorrectCost()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 3m);

        var cost = service.CalculateEstimatedCost(order.Id);
        Assert.Equal(1050m, cost); // 350 * 3
    }

    [Fact]
    public void GetWorkOrdersByStatus_ReturnsCorrectOrders()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        var order2 = service.CreateWorkOrder(vehicleId, mechanicId, "Bromsservice", 3m);
        service.StartWorkOrder(order2.Id);

        var pending = service.GetWorkOrdersByStatus(WorkOrderStatus.Pending);
        Assert.Single(pending);
    }

    private (WorkOrderService service, int vehicleId, int mech1Id, int mech2Id) CreateServiceWithTwoMechanics()
    {
        var workOrderRepo = new InMemoryWorkOrderRepository();
        var vehicleRepo = new InMemoryVehicleRepository();
        var mechanicRepo = new InMemoryMechanicRepository();
        var factory = new WorkOrderFactory();
        vehicleRepo.Add(new CarMechanic.Domain.Entities.Vehicle
        {
            Id = 1, LicensePlate = "ABC123", Brand = "Volvo", Model = "V70",
            Year = 2020, VehicleType = VehicleType.Car, OwnerId = 1, Mileage = 50000
        });
        mechanicRepo.Add(new CarMechanic.Domain.Entities.Mechanic
        {
            Id = 1, FirstName = "Erik", LastName = "Johansson",
            Specialty = MechanicSpecialty.Engine, HourlyRate = 350m
        });
        mechanicRepo.Add(new CarMechanic.Domain.Entities.Mechanic
        {
            Id = 2, FirstName = "Lars", LastName = "Andersen",
            Specialty = MechanicSpecialty.Electrical, HourlyRate = 300m
        });
        return (new WorkOrderService(workOrderRepo, vehicleRepo, mechanicRepo, factory), 1, 1, 2);
    }

    [Fact]
    public void ReassignMechanic_ValidData_UpdatesMechanicId()
    {
        var (service, vehicleId, mech1Id, mech2Id) = CreateServiceWithTwoMechanics();
        var order = service.CreateWorkOrder(vehicleId, mech1Id, "Oljebyte", 2m);
        service.ReassignMechanic(order.Id, mech2Id);
        var updated = service.GetWorkOrderById(order.Id);
        Assert.Equal(mech2Id, updated!.MechanicId);
    }

    [Fact]
    public void ReassignMechanic_CompletedWorkOrder_ThrowsInvalidOperationException()
    {
        var (service, vehicleId, mech1Id, mech2Id) = CreateServiceWithTwoMechanics();
        var order = service.CreateWorkOrder(vehicleId, mech1Id, "Oljebyte", 2m);
        service.StartWorkOrder(order.Id);
        service.CompleteWorkOrder(order.Id, 2m);
        Assert.Throws<InvalidOperationException>(() => service.ReassignMechanic(order.Id, mech2Id));
    }

    [Fact]
    public void ReassignMechanic_CancelledWorkOrder_ThrowsInvalidOperationException()
    {
        var (service, vehicleId, mech1Id, mech2Id) = CreateServiceWithTwoMechanics();
        var order = service.CreateWorkOrder(vehicleId, mech1Id, "Oljebyte", 2m);
        service.CancelWorkOrder(order.Id);
        Assert.Throws<InvalidOperationException>(() => service.ReassignMechanic(order.Id, mech2Id));
    }

    [Fact]
    public void ReassignMechanic_InvalidMechanic_ThrowsInvalidOperationException()
    {
        var (service, vehicleId, mech1Id, _) = CreateServiceWithTwoMechanics();
        var order = service.CreateWorkOrder(vehicleId, mech1Id, "Oljebyte", 2m);
        Assert.Throws<InvalidOperationException>(() => service.ReassignMechanic(order.Id, 999));
    }

    [Fact]
    public void GetPartUsageReport_CompletedOrderWithPart_ReturnsCorrectDict()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.StartWorkOrder(order.Id);
        service.AddPartToWorkOrder(order.Id, 5, 4, 100m);
        service.CompleteWorkOrder(order.Id, 2m);
        var report = service.GetPartUsageReport(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
        Assert.True(report.ContainsKey(5));
        Assert.Equal(4, report[5]);
    }

    [Fact]
    public void GetPartUsageReport_TwoOrdersSamePart_AggregatesQuantity()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var o1 = service.CreateWorkOrder(vehicleId, mechanicId, "Job1", 1m);
        service.StartWorkOrder(o1.Id);
        service.AddPartToWorkOrder(o1.Id, 7, 3, 50m);
        service.CompleteWorkOrder(o1.Id, 1m);
        var o2 = service.CreateWorkOrder(vehicleId, mechanicId, "Job2", 1m);
        service.StartWorkOrder(o2.Id);
        service.AddPartToWorkOrder(o2.Id, 7, 5, 50m);
        service.CompleteWorkOrder(o2.Id, 1m);
        var report = service.GetPartUsageReport(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
        Assert.Equal(8, report[7]);
    }

    [Fact]
    public void GetPartUsageReport_PendingOrderExcluded_ReturnsEmpty()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.AddPartToWorkOrder(order.Id, 9, 2, 100m);
        var report = service.GetPartUsageReport(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
        Assert.Empty(report);
    }

    [Fact]
    public void GetPartUsageReport_OutsideDateRange_ReturnsEmpty()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.StartWorkOrder(order.Id);
        service.AddPartToWorkOrder(order.Id, 9, 2, 100m);
        service.CompleteWorkOrder(order.Id, 2m);
        var report = service.GetPartUsageReport(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));
        Assert.Empty(report);
    }

    [Fact]
    public void AddPartToWorkOrder_ValidData_AddsPartToList()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.AddPartToWorkOrder(order.Id, 1, 2, 150m);
        var updated = service.GetWorkOrderById(order.Id);
        Assert.Single(updated!.Parts);
        Assert.Equal(2, updated.Parts[0].Quantity);
    }

    [Fact]
    public void AddPartToWorkOrder_ZeroQuantity_ThrowsArgumentException()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        Assert.Throws<ArgumentException>(() => service.AddPartToWorkOrder(order.Id, 1, 0, 150m));
    }

    [Fact]
    public void AddPartToWorkOrder_CompletedOrder_ThrowsInvalidOperationException()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        var order = service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.StartWorkOrder(order.Id);
        service.CompleteWorkOrder(order.Id, 2m);
        Assert.Throws<InvalidOperationException>(() => service.AddPartToWorkOrder(order.Id, 1, 2, 150m));
    }

    [Fact]
    public void GetWorkOrdersByVehicle_ReturnsCorrectOrders()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        service.CreateWorkOrder(vehicleId, mechanicId, "Bromsservice", 3m);
        var orders = service.GetWorkOrdersByVehicle(vehicleId);
        Assert.Equal(2, orders.Count);
    }

    [Fact]
    public void GetWorkOrdersByMechanic_ReturnsCorrectOrders()
    {
        var (service, vehicleId, mechanicId) = CreateServiceWithData();
        service.CreateWorkOrder(vehicleId, mechanicId, "Oljebyte", 2m);
        var orders = service.GetWorkOrdersByMechanic(mechanicId);
        Assert.Single(orders);
    }

    [Fact]
    public void GetWorkOrdersByMechanic_InvalidMechanic_ThrowsInvalidOperationException()
    {
        var (service, _, _) = CreateServiceWithData();
        Assert.Throws<InvalidOperationException>(() => service.GetWorkOrdersByMechanic(999));
    }
}
