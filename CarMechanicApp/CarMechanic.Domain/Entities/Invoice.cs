namespace CarMechanic.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime IssueDate { get; set; }
    public bool IsPaid { get; set; }
    public decimal DiscountPercent { get; set; }
}
