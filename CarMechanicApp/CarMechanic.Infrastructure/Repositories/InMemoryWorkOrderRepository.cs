using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Repositories;

public class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly List<WorkOrder> _workOrders = new();

    public WorkOrder? GetById(int id) => _workOrders.FirstOrDefault(wo => wo.Id == id);
    public List<WorkOrder> GetAll() => _workOrders.ToList();
    public List<WorkOrder> GetByVehicleId(int vehicleId) => _workOrders.Where(wo => wo.VehicleId == vehicleId).ToList();
    public List<WorkOrder> GetByMechanicId(int mechanicId) => _workOrders.Where(wo => wo.MechanicId == mechanicId).ToList();
    // BUG_TARGET: GetByStatus
    public List<WorkOrder> GetByStatus(WorkOrderStatus status) => _workOrders.Where(wo => wo.Status == status).ToList();
    public void Add(WorkOrder workOrder) => _workOrders.Add(workOrder);

    public void Update(WorkOrder workOrder)
    {
        var index = _workOrders.FindIndex(wo => wo.Id == workOrder.Id);
        if (index >= 0)
            _workOrders[index] = workOrder;
    }
}
