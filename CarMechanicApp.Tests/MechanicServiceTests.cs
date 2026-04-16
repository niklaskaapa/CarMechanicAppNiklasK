// Examination: Aron Åkesäter
// Generated: 2026-04-02
// Domain: CarMechanic

using CarMechanic.Application.Services;
using CarMechanic.Domain.Enums;
using CarMechanic.Infrastructure.Repositories;

namespace CarMechanic.Tests;

public class MechanicServiceTests
{
    private MechanicService CreateService()
    {
        var mechanicRepo = new InMemoryMechanicRepository();
        var workOrderRepo = new InMemoryWorkOrderRepository();
        return new MechanicService(mechanicRepo, workOrderRepo);
    }

    [Fact]
    public void HireMechanic_ValidData_ReturnsMechanic()
    {
        var service = CreateService();
        var mechanic = service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, 350m);

        Assert.Equal("Erik", mechanic.FirstName);
        Assert.Equal("Johansson", mechanic.LastName);
        Assert.Equal(MechanicSpecialty.Engine, mechanic.Specialty);
        Assert.Equal(350m, mechanic.HourlyRate);
    }

    [Fact]
    public void HireMechanic_EmptyName_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() => service.HireMechanic("", "Johansson", MechanicSpecialty.Engine, 350m));
    }

    [Fact]
    public void HireMechanic_NegativeRate_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() => service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, -100m));
    }

    [Fact]
    public void GetMechanicsBySpecialty_ReturnsCorrectMechanics()
    {
        var service = CreateService();
        service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, 350m);
        service.HireMechanic("Anna", "Svensson", MechanicSpecialty.Electrical, 300m);
        service.HireMechanic("Lars", "Nilsson", MechanicSpecialty.Engine, 400m);

        var engineMechanics = service.GetMechanicsBySpecialty(MechanicSpecialty.Engine);
        Assert.Equal(2, engineMechanics.Count);
    }

    [Fact]
    public void CalculateLabourCost_ReturnsCorrectCost()
    {
        var service = CreateService();
        var mechanic = service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, 350m);

        var cost = service.CalculateLabourCost(mechanic.Id, 3m);
        Assert.Equal(1050m, cost);
    }

    [Fact]
    public void CalculateLabourCost_ZeroHours_ThrowsArgumentException()
    {
        var service = CreateService();
        var mechanic = service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, 350m);

        Assert.Throws<ArgumentException>(() => service.CalculateLabourCost(mechanic.Id, 0m));
    }

    [Fact]
    public void GetMechanicFullName_ReturnsCorrectName()
    {
        var service = CreateService();
        var mechanic = service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, 350m);

        var name = service.GetMechanicFullName(mechanic.Id);
        Assert.Equal("Erik Johansson", name);
    }

    [Fact]
    public void GetTotalMechanics_ReturnsCorrectCount()
    {
        var service = CreateService();
        service.HireMechanic("Erik", "Johansson", MechanicSpecialty.Engine, 350m);
        service.HireMechanic("Anna", "Svensson", MechanicSpecialty.Electrical, 300m);

        Assert.Equal(2, service.GetTotalMechanics());
    }
}
