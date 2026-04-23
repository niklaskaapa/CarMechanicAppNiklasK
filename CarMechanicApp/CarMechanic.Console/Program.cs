using CarMechanic.Application.Services;
using CarMechanic.Domain.Enums;
using CarMechanic.Infrastructure.Factories;
using CarMechanic.Infrastructure.Repositories;

namespace CarMechanic.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var vehicleRepo = new InMemoryVehicleRepository();
        var customerRepo = new InMemoryCustomerRepository();
        var mechanicRepo = new InMemoryMechanicRepository();
        var workOrderRepo = new InMemoryWorkOrderRepository();
        var partRepo = new InMemoryPartRepository();
        var invoiceRepo = new InMemoryInvoiceRepository();

        var workOrderFactory = new WorkOrderFactory();

        var vehicleService = new VehicleService(vehicleRepo, customerRepo);
        var mechanicService = new MechanicService(mechanicRepo, workOrderRepo);
        var workOrderService = new WorkOrderService(workOrderRepo, vehicleRepo, mechanicRepo, workOrderFactory);
        var partService = new PartService(partRepo);
        var invoiceService = new InvoiceService(invoiceRepo, workOrderRepo, mechanicRepo);

        bool running = true;

        while (running)
        {
            System.Console.WriteLine("\n=== Bilverkstaden ===");
            System.Console.WriteLine("1. Registrera kund");
            System.Console.WriteLine("2. Registrera fordon");
            System.Console.WriteLine("3. Anställ mekaniker");
            System.Console.WriteLine("4. Lägg till reservdel");
            System.Console.WriteLine("5. Skapa arbetsorder");
            System.Console.WriteLine("6. Slutför arbetsorder");
            System.Console.WriteLine("7. Skapa faktura");
            System.Console.WriteLine("8. Visa alla fordon");
            System.Console.WriteLine("9. Visa mekaniker");
            System.Console.WriteLine("10. Visa mekanikers intäkt");
            System.Console.WriteLine("11. Avsluta");
            System.Console.Write("\nVälj alternativ: ");

            var choice = System.Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterCustomer(customerRepo);
                    break;
                case "2":
                    RegisterVehicle(vehicleService);
                    break;
                case "3":
                    HireMechanic(mechanicService);
                    break;
                case "4":
                    AddPart(partService);
                    break;
                case "5":
                    CreateWorkOrder(workOrderService);
                    break;
                case "6":
                    CompleteWorkOrder(workOrderService);
                    break;
                case "7":
                    CreateInvoice(invoiceService);
                    break;
                case "8":
                    ShowVehicles(vehicleService);
                    break;
                case "9":
                    ShowMechanics(mechanicService);
                    break;
                case "10":
                    ShowMechanicRevenue(mechanicService);
                    break;
                case "11":
                    running = false;
                    break;
                default:
                    System.Console.WriteLine("Ogiltigt val.");
                    break;
            }
        }
    }

    

    private static void RegisterCustomer(InMemoryCustomerRepository repo)
    {
        System.Console.Write("Förnamn: ");
        var first = System.Console.ReadLine() ?? "";
        System.Console.Write("Efternamn: ");
        var last = System.Console.ReadLine() ?? "";
        System.Console.Write("E-post: ");
        var email = System.Console.ReadLine() ?? "";
        System.Console.Write("Telefon: ");
        var phone = System.Console.ReadLine() ?? "";

        var customer = new CarMechanic.Domain.Entities.Customer
        {
            Id = repo.GetAll().Count + 1,
            FirstName = first,
            LastName = last,
            Email = email,
            PhoneNumber = phone,
            RegistrationDate = DateTime.Now
        };
        repo.Add(customer);
        System.Console.WriteLine($"Kund '{customer.FullName}' registrerad med ID {customer.Id}.");
    }

    private static void RegisterVehicle(VehicleService vehicleService)
    {
        System.Console.Write("Registreringsnummer: ");
        var plate = System.Console.ReadLine() ?? "";
        System.Console.Write("Märke: ");
        var brand = System.Console.ReadLine() ?? "";
        System.Console.Write("Modell: ");
        var model = System.Console.ReadLine() ?? "";
        System.Console.Write("Årsmodell: ");
        var year = int.Parse(System.Console.ReadLine() ?? "2020");
        System.Console.Write("Ägar-ID: ");
        var ownerId = int.Parse(System.Console.ReadLine() ?? "1");
        System.Console.Write("Mätarställning (km): ");
        var mileage = int.Parse(System.Console.ReadLine() ?? "0");

        var vehicle = vehicleService.RegisterVehicle(plate, brand, model, year, ownerId, VehicleType.Car, mileage);
        System.Console.WriteLine($"Fordon '{vehicle.Brand} {vehicle.Model}' registrerat med ID {vehicle.Id}.");
    }

    private static void HireMechanic(MechanicService mechanicService)
    {
        System.Console.Write("Förnamn: ");
        var first = System.Console.ReadLine() ?? "";
        System.Console.Write("Efternamn: ");
        var last = System.Console.ReadLine() ?? "";
        System.Console.Write("Specialitet (Engine/Electrical/Body/General): ");
        var specStr = System.Console.ReadLine() ?? "General";
        if (!Enum.TryParse<MechanicSpecialty>(specStr, true, out var specialty))
            specialty = MechanicSpecialty.General;
        System.Console.Write("Timlön: ");
        var rate = decimal.Parse(System.Console.ReadLine() ?? "350");

        var mechanic = mechanicService.HireMechanic(first, last, specialty, rate);
        System.Console.WriteLine($"Mekaniker '{mechanic.FullName}' anställd med ID {mechanic.Id}. Anställningsdatum är: {mechanic.HireDate}");
    }

    private static void AddPart(PartService partService)
    {
        System.Console.Write("Namn: ");
        var name = System.Console.ReadLine() ?? "";
        System.Console.Write("Artikelnummer: ");
        var partNumber = System.Console.ReadLine() ?? "";
        System.Console.Write("Pris: ");
        var price = decimal.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Antal i lager: ");
        var stock = int.Parse(System.Console.ReadLine() ?? "0");

        var part = partService.AddPart(name, partNumber, price, stock, PartCategory.Engine);
        System.Console.WriteLine($"Reservdel '{part.Name}' tillagd med ID {part.Id}.");
    }

    private static void CreateWorkOrder(WorkOrderService workOrderService)
    {
        System.Console.Write("Fordons-ID: ");
        var vehicleId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Mekaniker-ID: ");
        var mechanicId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Beskrivning: ");
        var description = System.Console.ReadLine() ?? "";
        System.Console.Write("Uppskattade timmar: ");
        var hours = decimal.Parse(System.Console.ReadLine() ?? "1");

        var order = workOrderService.CreateWorkOrder(vehicleId, mechanicId, description, hours);
        System.Console.WriteLine($"Arbetsorder #{order.Id} skapad.");
    }

    private static void CompleteWorkOrder(WorkOrderService workOrderService)
    {
        System.Console.Write("Arbetsorder-ID: ");
        var orderId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Faktiska timmar: ");
        var hours = decimal.Parse(System.Console.ReadLine() ?? "1");

        workOrderService.CompleteWorkOrder(orderId, hours);
        System.Console.WriteLine($"Arbetsorder #{orderId} slutförd.");
    }

    private static void CreateInvoice(InvoiceService invoiceService)
    {
        System.Console.Write("Arbetsorder-ID: ");
        var orderId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Kund-ID: ");
        var customerId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Rabatt (%): ");
        var discount = decimal.Parse(System.Console.ReadLine() ?? "0");

        var invoice = invoiceService.CreateInvoice(orderId, customerId, discount);
        System.Console.WriteLine($"Faktura #{invoice.Id}: {invoice.TotalAmount:C}");
    }

    private static void ShowVehicles(VehicleService vehicleService)
    {
        var count = vehicleService.GetVehicleCount();
        if (count == 0)
        {
            System.Console.WriteLine("Inga fordon registrerade.");
            return;
        }
        System.Console.WriteLine($"Totalt antal fordon: {count}");
    }

    private static void ShowMechanics(MechanicService mechanicService)
    {
        var count = mechanicService.GetTotalMechanics();
        if (count == 0)
        {
            System.Console.WriteLine("Inga mekaniker anställda.");
            return;
        }
        System.Console.WriteLine($"Totalt antal mekaniker: {count}");
    }

    private static void ShowMechanicRevenue(MechanicService mechanicService)
    {
        while (true)
        {
            System.Console.Write("Ange mekaniker-ID: ");

            if (!int.TryParse(System.Console.ReadLine(), out int mechanicId) || mechanicId <= 0)
            {
                System.Console.WriteLine("Ogiltigt ID, försök igen.");
                continue;
            }

            try
            {
                var revenue = mechanicService.GetMechanicRevenue(mechanicId);
                var name = mechanicService.GetMechanicFullName(mechanicId);

                System.Console.WriteLine($"Mekaniker: {name}");
                System.Console.WriteLine($"Total intäkt: {revenue:F2} kr");
                return; 
            }
            catch (InvalidOperationException ex)
            {
                System.Console.WriteLine(ex.Message);
                return;
                
            }
        }

    }


}
