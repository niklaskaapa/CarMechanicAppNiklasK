using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Application.Services;

public class MechanicService
{
    private readonly IMechanicRepository _mechanicRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private int _nextId = 1;

    public MechanicService(IMechanicRepository mechanicRepository, IWorkOrderRepository workOrderRepository)
    {
        _mechanicRepository = mechanicRepository;
        _workOrderRepository = workOrderRepository;
    }

    // BUG_TARGET: HireMechanic
    public Mechanic HireMechanic(string firstName, string lastName, MechanicSpecialty specialty, decimal hourlyRate)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.");
        if (hourlyRate <= 0)
            throw new ArgumentException("Hourly rate must be positive.");

        var mechanic = new Mechanic
        {
            Id = _nextId++,
            FirstName = firstName,
            LastName = lastName,
            Specialty = specialty,
            HourlyRate = hourlyRate,
            HireDate = DateTime.Now
        };

        _mechanicRepository.Add(mechanic);
        return mechanic;
    }

    // BUG_TARGET: CalculateLabourCost
    public decimal CalculateLabourCost(int mechanicId, decimal hours)
    {
        if (hours <= 0)
            throw new ArgumentException("Hours must be positive.");

        var mechanic = _mechanicRepository.GetById(mechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        return Math.Round(mechanic.HourlyRate * hours, 2);
    }

    // MISSING_TARGET: GetMechanicWorkload
    public int GetMechanicWorkload(int mechanicId)
    {
        var mechanic = _mechanicRepository.GetById(mechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        return _workOrderRepository.GetByMechanicId(mechanicId)
            .Count(wo => wo.Status == WorkOrderStatus.InProgress || wo.Status == WorkOrderStatus.Pending);
    }

    // BUG_TARGET: GetMechanicsBySpecialty
    public List<Mechanic> GetMechanicsBySpecialty(MechanicSpecialty specialty)
    {
        return _mechanicRepository.GetBySpecialty(specialty);
    }

    // MISSING_TARGET: GetAvailableMechanics
    public List<Mechanic> GetAvailableMechanics(int maxWorkload)
    {
        if (maxWorkload < 0)
            throw new ArgumentException("Max workload cannot be negative.");

        var mechanics = _mechanicRepository.GetAll();
        return mechanics
            .Where(m =>
            {
                var activeOrders = _workOrderRepository.GetByMechanicId(m.Id)
                    .Count(wo => wo.Status == WorkOrderStatus.InProgress || wo.Status == WorkOrderStatus.Pending);
                return activeOrders <= maxWorkload;
            })
            .ToList();
    }

    // BUG_TARGET: GetMechanicEfficiency
    public decimal GetMechanicEfficiency(int mechanicId)
    {
        var mechanic = _mechanicRepository.GetById(mechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        var completedOrders = _workOrderRepository.GetByMechanicId(mechanicId)
            .Where(wo => wo.Status == WorkOrderStatus.Completed)
            .ToList();

        if (completedOrders.Count == 0)
            return 0;

        var totalEstimated = completedOrders.Sum(wo => wo.EstimatedHours);
        var totalActual = completedOrders.Sum(wo => wo.ActualHours);

        if (totalActual == 0)
            return 0;

        return Math.Round(totalEstimated / totalActual * 100, 2);
    }

    // MISSING_TARGET: GetMechanicRevenue
    public decimal GetMechanicRevenue(int mechanicId)       // Hämta Mekanikers Intäkter plus eventuella intäkter för reservdelar.
    {
        var mechanic = _mechanicRepository.GetById(mechanicId);
        if (mechanic == null)
        {
            throw new InvalidOperationException("Mechanic not found.");
        }

        var compleatedOrders = _workOrderRepository
            .GetByMechanicId(mechanicId)
            .Where(wo => wo.Status == WorkOrderStatus.Completed);

        var revenue = compleatedOrders.Sum(wo =>
        (wo.ActualHours * mechanic.HourlyRate) +
        wo.Parts.Sum(p => p.TotalPrice));


        return Math.Round(revenue, 2);

    }

    // BUG_TARGET: GetTotalMechanics
    public int GetTotalMechanics()
    {
        return _mechanicRepository.GetAll().Count;
    }

    // MISSING_TARGET: GetMechanicFullName
    public string GetMechanicFullName(int mechanicId)
    {
        var mechanic = _mechanicRepository.GetById(mechanicId);

        return mechanic.FullName;
       
    }

    public Mechanic? GetMechanicById(int id)
    {
        return _mechanicRepository.GetById(id);
    }
}
