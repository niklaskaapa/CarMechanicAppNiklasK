using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Entities;

public class WorkOrder
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int MechanicId { get; set; }
    public string Description { get; set; } = string.Empty;
    public WorkOrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public List<WorkOrderPart> Parts { get; set; } = new();
}
