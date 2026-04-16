using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Factories;

public class WorkOrderFactory : IWorkOrderFactory
{
    // BUG_TARGET: CreateWorkOrder
    public WorkOrder CreateWorkOrder(int vehicleId, int mechanicId, string description, decimal estimatedHours)
    {
        return new WorkOrder
        {
            VehicleId = vehicleId,
            MechanicId = mechanicId,
            Description = description,
            Status = WorkOrderStatus.Pending,
            CreatedDate = DateTime.Now,
            EstimatedHours = estimatedHours,
            ActualHours = 0
        };
    }
}
