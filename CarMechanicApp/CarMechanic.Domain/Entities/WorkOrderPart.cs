namespace CarMechanic.Domain.Entities;

public class WorkOrderPart
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}
