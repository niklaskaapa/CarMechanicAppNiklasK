using CarMechanic.Domain.Entities;
using CarMechanic.Domain.Interfaces;

namespace CarMechanic.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public Customer? GetById(int id) => _customers.FirstOrDefault(c => c.Id == id);
    public List<Customer> GetAll() => _customers.ToList();
    public Customer? GetByEmail(string email) => _customers.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    public void Add(Customer customer) => _customers.Add(customer);

    public void Update(Customer customer)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0)
            _customers[index] = customer;
    }
}
