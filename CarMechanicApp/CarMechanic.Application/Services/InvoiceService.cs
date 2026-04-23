using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Enums;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Application.Services;

public class InvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IMechanicRepository _mechanicRepository;
    private int _nextId = 1;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IWorkOrderRepository workOrderRepository,
        IMechanicRepository mechanicRepository)
    {
        _invoiceRepository = invoiceRepository;
        _workOrderRepository = workOrderRepository;
        _mechanicRepository = mechanicRepository;
    }

    // BUG_TARGET: CreateInvoice
    public Invoice CreateInvoice(int workOrderId, int customerId, decimal discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException("Discount must be between 0 and 100.");

        var workOrder = _workOrderRepository.GetById(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException("Work order not found.");
        if (workOrder.Status != WorkOrderStatus.Completed)
            throw new InvalidOperationException("Work order must be completed to create an invoice.");

        var mechanic = _mechanicRepository.GetById(workOrder.MechanicId);
        if (mechanic == null)
            throw new InvalidOperationException("Mechanic not found.");

        var laborCost = Math.Round(workOrder.ActualHours * mechanic.HourlyRate, 2);
        var partsCost = workOrder.Parts.Sum(p => p.TotalPrice);
        var subtotal = laborCost + partsCost;
        var discount = Math.Round(subtotal * (discountPercent / 100m), 2);
        var totalAmount = subtotal - discount;

        var invoice = new Invoice
        {
            Id = _nextId++,
            WorkOrderId = workOrderId,
            CustomerId = customerId,
            LaborCost = laborCost,
            PartsCost = partsCost,
            TotalAmount = totalAmount,
            IssueDate = DateTime.Now,
            IsPaid = false,
            DiscountPercent = discountPercent
        };

        _invoiceRepository.Add(invoice);
        return invoice;
    }

    // BUG_TARGET: MarkAsPaid
    public void MarkAsPaid(int invoiceId)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("Invoice not found.");
        if (invoice.IsPaid)
            throw new InvalidOperationException("Invoice is already paid.");

        invoice.IsPaid = true;
        _invoiceRepository.Update(invoice);
    }

    // MISSING_TARGET: GetUnpaidInvoices
    public List<Invoice> GetUnpaidInvoices()
    {
        return _invoiceRepository.GetUnpaid();
    }

    // BUG_TARGET: GetRevenueByPeriod
    public decimal GetRevenueByPeriod(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date must be before to date.");

        var invoices = _invoiceRepository.GetByDateRange(from, to);
        return invoices.Where(i => i.IsPaid).Sum(i => i.TotalAmount);
    }

    // MISSING_TARGET: GetInvoicesByCustomer
    public List<Invoice> GetInvoicesByCustomer(int customerId)
    {
        
        return _invoiceRepository.GetByCustomerId(customerId);

    }

    // BUG_TARGET: CalculateOutstandingBalance
    public decimal CalculateOutstandingBalance()
    {
        return _invoiceRepository.GetUnpaid().Sum(i => i.TotalAmount);
    }

    // MISSING_TARGET: ApplyDiscount
    public decimal ApplyDiscount(int invoiceId, decimal discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException("Discount must be between 0 and 100.");

        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("Invoice not found.");
        if (invoice.IsPaid)
            throw new InvalidOperationException("Cannot apply discount to a paid invoice.");

        var subtotal = invoice.LaborCost + invoice.PartsCost;
        var discount = Math.Round(subtotal * (discountPercent / 100m), 2);
        invoice.TotalAmount = subtotal - discount;
        invoice.DiscountPercent = discountPercent;
        _invoiceRepository.Update(invoice);

        return invoice.TotalAmount;
    }

    // BUG_TARGET: GetAverageInvoiceAmount
    public decimal GetAverageInvoiceAmount()
    {
        var invoices = _invoiceRepository.GetAll();
        if (invoices.Count == 0)
            return 0;

        return Math.Round(invoices.Average(i => i.TotalAmount), 2);
    }

    // MISSING_TARGET: GetTotalRevenue
    public decimal GetTotalRevenue()
    {
        return _invoiceRepository.GetAll().Where(i => i.IsPaid).Sum(i => i.TotalAmount);
    }

    // MISSING_TARGET: GenerateInvoiceSummary
    public string GenerateInvoiceSummary(int invoiceId)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("Invoice not found.");

        var workOrder = _workOrderRepository.GetById(invoice.WorkOrderId);
        var mechanic = workOrder != null ? _mechanicRepository.GetById(workOrder.MechanicId) : null;
        var partCount = workOrder?.Parts.Count ?? 0;

        var status = invoice.IsPaid ? "Betald" : "Obetald";
        var mechanicName = mechanic != null
            ? $"{mechanic.FirstName} {mechanic.LastName}"
            : "Okänd";

        return $"Faktura {invoice.Id}: {mechanicName}, " +
               $"Arbete: {invoice.LaborCost:F2} kr, Delar: {invoice.PartsCost:F2} kr ({partCount} st), " +
               $"Rabatt: {invoice.DiscountPercent:F1}%, Totalt: {invoice.TotalAmount:F2} kr [{status}]";
    }

    // MISSING_TARGET: CalculateCustomerTotal
    public (decimal PaidTotal, decimal UnpaidTotal) CalculateCustomerTotal(int customerId)
    {
        var invoices = _invoiceRepository.GetByCustomerId(customerId);
        var paidTotal = invoices.Where(i => i.IsPaid).Sum(i => i.TotalAmount);
        var unpaidTotal = invoices.Where(i => !i.IsPaid).Sum(i => i.TotalAmount);
        return (Math.Round(paidTotal, 2), Math.Round(unpaidTotal, 2));
    }

    public Invoice? GetInvoiceById(int id)
    {
        return _invoiceRepository.GetById(id);
    }
}
