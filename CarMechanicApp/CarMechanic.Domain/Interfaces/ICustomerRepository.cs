using CarMechanic.Domain.Entities;

namespace CarMechanic.Domain.Interfaces;

public interface ICustomerRepository
{
    Customer? GetById(int id);
    List<Customer> GetAll();
    Customer? GetByEmail(string email);
    void Add(Customer customer);
    void Update(Customer customer);
}
