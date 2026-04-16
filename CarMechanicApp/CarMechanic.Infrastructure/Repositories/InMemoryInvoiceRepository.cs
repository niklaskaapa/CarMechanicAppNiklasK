using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Repositories;

public class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly List<Invoice> _invoices = new();

    public Invoice? GetById(int id) => _invoices.FirstOrDefault(i => i.Id == id);
    public List<Invoice> GetAll() => _invoices.ToList();
    public List<Invoice> GetByCustomerId(int customerId) => _invoices.Where(i => i.CustomerId == customerId).ToList();
    // BUG_TARGET: GetByDateRange
    public List<Invoice> GetByDateRange(DateTime from, DateTime to) => _invoices.Where(i => i.IssueDate >= from && i.IssueDate <= to).ToList();
    // BUG_TARGET: GetUnpaid
    public List<Invoice> GetUnpaid() => _invoices.Where(i => !i.IsPaid).ToList();
    public void Add(Invoice invoice) => _invoices.Add(invoice);

    public void Update(Invoice invoice)
    {
        var index = _invoices.FindIndex(i => i.Id == invoice.Id);
        if (index >= 0)
            _invoices[index] = invoice;
    }
}
