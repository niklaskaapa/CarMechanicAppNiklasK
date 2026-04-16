using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Interfaces;

public interface IWorkOrderFactory
{
    WorkOrder CreateWorkOrder(int vehicleId, int mechanicId, string description, decimal estimatedHours);
}
