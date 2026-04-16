using CarMechanic.Domain.Entities;

namespace CarMechanic.Domain.Interfaces;

public interface IInvoiceRepository
{
    Invoice? GetById(int id);
    List<Invoice> GetAll();
    List<Invoice> GetByCustomerId(int customerId);
    List<Invoice> GetByDateRange(DateTime from, DateTime to);
    List<Invoice> GetUnpaid();
    void Add(Invoice invoice);
    void Update(Invoice invoice);
}
