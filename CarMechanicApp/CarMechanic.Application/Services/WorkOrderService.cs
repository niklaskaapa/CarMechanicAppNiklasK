using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Application.Services;

public class WorkOrderService
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMechanicRepository _mechanicRepository;
    private readonly IWorkOrderFactory _workOrderFactory;
    private int _nextId = 1;

    public WorkOrderService(
        IWorkOrderRepository workOrderRepository,
        IVehicleRepository vehicleRepository,
        IMechanicRepository mechanicRepository,
        IWorkOrderFactory workOrderFactory)
    {
        _workOrderRepository = workOrderRepository;
        _vehicleRepository = vehicleRepository;
        _mechanicRepository = mechanicRepository;
        _workOrderFactory = workOrderFactory;
    }

    // BUG_TARGET: CreateWorkOrder
    public WorkOrder CreateWorkOrder(int vehicleId, int mechanicId, string description, decimal estimatedHours)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.");
        if (estimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive.");

        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found.");

        var mechanic = _mechanicRepository.GetById(mechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        var workOrder = _workOrderFactory.CreateWorkOrder(vehicleId, mechanicId, description, estimatedHours);
        workOrder.Id = _nextId++;
        _workOrderRepository.Add(workOrder);
        return workOrder;
    }

    // BUG_TARGET: CompleteWorkOrder
    public void CompleteWorkOrder(int workOrderId, decimal actualHours)
    {
        if (actualHours <= 0)
            throw new ArgumentException("Actual hours must be positive.");

        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");
        if (workOrder.Status == WorkOrderStatus.Completed)
            throw new InvalidOperationException("Work order is already completed.");
        if (workOrder.Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled work order.");

        workOrder.Status = WorkOrderStatus.Completed;
        workOrder.ActualHours = actualHours;
        workOrder.CompletedDate = DateTime.Now;
        _workOrderRepository.Update(workOrder);
    }

    // MISSING_TARGET: StartWorkOrder
    public void StartWorkOrder(int workOrderId)
    {
        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");
        if (workOrder.Status != WorkOrderStatus.Pending)
            throw new InvalidOperationException("Only pending work orders can be started.");

        workOrder.Status = WorkOrderStatus.InProgress;
        _workOrderRepository.Update(workOrder);
    }

    // BUG_TARGET: CancelWorkOrder
    public void CancelWorkOrder(int workOrderId)
    {
        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");
        if (workOrder.Status == WorkOrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed work order.");

        workOrder.Status = WorkOrderStatus.Cancelled;
        _workOrderRepository.Update(workOrder);
    }

    // MISSING_TARGET: CalculateEstimatedCost
    public decimal CalculateEstimatedCost(int workOrderId)
    {
        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");

        var mechanic = _mechanicRepository.GetById(workOrder.MechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        var laborCost = workOrder.EstimatedHours * mechanic.HourlyRate;
        var partsCost = workOrder.Parts.Sum(p => p.TotalPrice);

        return Math.Round(laborCost + partsCost, 2);
    }

    // BUG_TARGET: GetOverdueOrders
    public List<WorkOrder> GetOverdueOrders(int maxDays)
    {
        if (maxDays <= 0)
            throw new ArgumentException("Max days must be positive.");

        return _workOrderRepository.GetAll()
            .Where(wo => wo.Status != WorkOrderStatus.Completed && wo.Status != WorkOrderStatus.Cancelled)
            .Where(wo => (DateTime.Now - wo.CreatedDate).TotalDays > maxDays)
            .ToList();
    }

    // MISSING_TARGET: GetWorkOrdersByMechanic
    public List<WorkOrder> GetWorkOrdersByMechanic(int mechanicId)
    {
        var mechanic = _mechanicRepository.GetById(mechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        return _workOrderRepository.GetByMechanicId(mechanicId);
    }

    // BUG_TARGET: GetWorkOrdersByStatus
    public List<WorkOrder> GetWorkOrdersByStatus(WorkOrderStatus status)
    {
        return _workOrderRepository.GetByStatus(status);
    }

    // MISSING_TARGET: AddPartToWorkOrder
    public void AddPartToWorkOrder(int workOrderId, int partId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");

        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");
        if (workOrder.Status == WorkOrderStatus.Completed || workOrder.Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot modify a completed or cancelled work order.");

        var part = new WorkOrderPart
        {
            Id = workOrder.Parts.Count + 1,
            WorkOrderId = workOrderId,
            PartId = partId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        workOrder.Parts.Add(part);
        _workOrderRepository.Update(workOrder);
    }

    // BUG_TARGET: GetAverageCompletionTime
    public decimal GetAverageCompletionTime()
    {
        var completed = _workOrderRepository.GetAll()
            .Where(wo => wo.Status == WorkOrderStatus.Completed)
            .ToList();

        if (completed.Count == 0)
            return 0;

        return Math.Round(completed.Average(wo => wo.ActualHours), 2);
    }

    // MISSING_TARGET: GetWorkOrdersByVehicle
    public List<WorkOrder> GetWorkOrdersByVehicle(int vehicleId)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found.");

        return _workOrderRepository.GetByVehicleId(vehicleId);
    }

    public WorkOrder? GetWorkOrderById(int id)
    {
        return _workOrderRepository.GetById(id);
    }

    // MISSING_TARGET: ReassignMechanic
    public void ReassignMechanic(int workOrderId, int newMechanicId)
    {
        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");
        if (workOrder.Status == WorkOrderStatus.Completed || workOrder.Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot reassign a completed or cancelled work order.");

        var newMechanic = _mechanicRepository.GetById(newMechanicId);
        if (newMechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        workOrder.MechanicId = newMechanic.Id;
        // update not persisted
        _workOrderRepository.Update(workOrder);
    }

    // MISSING_TARGET: GetPartUsageReport
    public Dictionary<int, int> GetPartUsageReport(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date must be before to date.");

        var orders = _workOrderRepository.GetAll()
            .Where(wo => wo.Status == WorkOrderStatus.Completed
                         && wo.CompletedDate.HasValue
                         && wo.CompletedDate.Value >= from 
                         && wo.CompletedDate.Value <= to)
            .ToList();

        var report = new Dictionary<int, int>();
        foreach (var order in orders)
        {
            foreach (var part in order.Parts)
            {
                if (report.ContainsKey(part.PartId))
                    report[part.PartId] += part.Quantity;
                else
                    report[part.PartId] = part.Quantity;
            }
        }

        return report;
    }
}
