using CarMechanic.Domain.Enums;

namespace CarMechanic.Domain.Entities;

public class Mechanic
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public MechanicSpecialty Specialty { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime HireDate { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
