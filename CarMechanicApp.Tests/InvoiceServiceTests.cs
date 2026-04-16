// Examination: Aron Åkesäter
// Generated: 2026-04-02
// Domain: CarMechanic

using CarMechanic.Application.Services;
using CarMechanic.Domain.Enums;
using CarMechanic.Infrastructure.Repositories;

namespace CarMechanic.Tests;

public class InvoiceServiceTests
{
    private (InvoiceService service, int workOrderId, int customerId) CreateServiceWithData()
    {
        var invoiceRepo = new InMemoryInvoiceRepository();
        var workOrderRepo = new InMemoryWorkOrderRepository();
        var mechanicRepo = new InMemoryMechanicRepository();

        var mechanic = new CarMechanic.Domain.Entities.Mechanic
        {
            Id = 1,
            FirstName = "Erik",
            LastName = "Johansson",
            Specialty = MechanicSpecialty.Engine,
            HourlyRate = 350m
        };
        mechanicRepo.Add(mechanic);

        var workOrder = new CarMechanic.Domain.Entities.WorkOrder
        {
            Id = 1,
            VehicleId = 1,
            MechanicId = 1,
            Description = "Oljebyte",
            Status = WorkOrderStatus.Completed,
            ActualHours = 2,
            EstimatedHours = 3,
            CreatedDate = DateTime.Now.AddDays(-1),
            CompletedDate = DateTime.Now
        };
        workOrderRepo.Add(workOrder);

        var service = new InvoiceService(invoiceRepo, workOrderRepo, mechanicRepo);
        return (service, workOrder.Id, 1);
    }

    [Fact]
    public void CreateInvoice_ValidWorkOrder_ReturnsInvoice()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);

        Assert.Equal(workOrderId, invoice.WorkOrderId);
        Assert.Equal(700m, invoice.LaborCost); // 350 * 2 hours
        Assert.Equal(customerId, invoice.CustomerId);
    }

    [Fact]
    public void CreateInvoice_InvalidWorkOrder_ThrowsInvalidOperationException()
    {
        var (service, _, customerId) = CreateServiceWithData();
        Assert.Throws<InvalidOperationException>(() => service.CreateInvoice(999, customerId, 0));
    }

    [Fact]
    public void CreateInvoice_WithDiscount_ReducesTotal()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 10);

        Assert.Equal(630m, invoice.TotalAmount); // (700 - 10%) = 630
    }

    [Fact]
    public void MarkAsPaid_SetsIsPaidTrue()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);

        var updated = service.GetInvoiceById(invoice.Id);
        Assert.True(updated!.IsPaid);
    }

    [Fact]
    public void MarkAsPaid_AlreadyPaid_ThrowsInvalidOperationException()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);
        Assert.Throws<InvalidOperationException>(() => service.MarkAsPaid(invoice.Id));
    }

    [Fact]
    public void GetUnpaidInvoices_ReturnsCorrectInvoices()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);

        var unpaid = service.GetUnpaidInvoices();
        Assert.Single(unpaid);
    }

    [Fact]
    public void GetTotalRevenue_ReturnsCorrectAmount()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);

        var revenue = service.GetTotalRevenue();
        Assert.Equal(700m, revenue);
    }

    [Fact]
    public void CalculateOutstandingBalance_ReturnsCorrectAmount()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);

        var balance = service.CalculateOutstandingBalance();
        Assert.Equal(700m, balance);
    }

    [Fact]
    public void GetAverageInvoiceAmount_ReturnsCorrectAverage()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);

        var average = service.GetAverageInvoiceAmount();
        Assert.Equal(700m, average);
    }

    [Fact]
    public void ApplyDiscount_ReducesTotalAmount()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        var newTotal = service.ApplyDiscount(invoice.Id, 20);

        Assert.Equal(560m, newTotal); // 700 - 20% = 560
    }

    [Fact]
    public void ApplyDiscount_PaidInvoice_ThrowsInvalidOperationException()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);

        Assert.Throws<InvalidOperationException>(() => service.ApplyDiscount(invoice.Id, 10));
    }

    [Fact]
    public void GetInvoicesByCustomer_ReturnsCorrectInvoices()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);

        var invoices = service.GetInvoicesByCustomer(customerId);
        Assert.Single(invoices);
    }

    [Fact]
    public void GetRevenueByPeriod_ReturnsCorrectAmount()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);

        var revenue = service.GetRevenueByPeriod(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
        Assert.Equal(700m, revenue);
    }

    [Fact]
    public void GenerateInvoiceSummary_ValidInvoice_ContainsMechanicName()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        var summary = service.GenerateInvoiceSummary(invoice.Id);
        Assert.Contains("Erik Johansson", summary);
    }

    [Fact]
    public void GenerateInvoiceSummary_UnpaidInvoice_ContainsObetald()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        var summary = service.GenerateInvoiceSummary(invoice.Id);
        Assert.Contains("Obetald", summary);
    }

    [Fact]
    public void GenerateInvoiceSummary_PaidInvoice_ContainsBetald()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);
        var summary = service.GenerateInvoiceSummary(invoice.Id);
        Assert.Contains("Betald", summary);
    }

    [Fact]
    public void GenerateInvoiceSummary_InvalidInvoice_ThrowsInvalidOperationException()
    {
        var (service, _, _) = CreateServiceWithData();
        Assert.Throws<InvalidOperationException>(() => service.GenerateInvoiceSummary(999));
    }

    [Fact]
    public void CalculateCustomerTotal_AllUnpaid_ReturnsZeroPaidCorrectUnpaid()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);
        var (paid, unpaid) = service.CalculateCustomerTotal(customerId);
        Assert.Equal(0m, paid);
        Assert.Equal(700m, unpaid);
    }

    [Fact]
    public void CalculateCustomerTotal_AllPaid_ReturnsCorrectPaidZeroUnpaid()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);
        var (paid, unpaid) = service.CalculateCustomerTotal(customerId);
        Assert.Equal(700m, paid);
        Assert.Equal(0m, unpaid);
    }

    [Fact]
    public void CalculateCustomerTotal_NoInvoices_ReturnsBothZero()
    {
        var (service, _, customerId) = CreateServiceWithData();
        var (paid, unpaid) = service.CalculateCustomerTotal(customerId);
        Assert.Equal(0m, paid);
        Assert.Equal(0m, unpaid);
    }

    [Fact]
    public void CalculateCustomerTotal_DifferentCustomer_ReturnsBothZero()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);
        var (paid, unpaid) = service.CalculateCustomerTotal(999);
        Assert.Equal(0m, paid);
        Assert.Equal(0m, unpaid);
    }

    [Fact]
    public void GetUnpaidInvoices_AfterPayingAll_ReturnsEmpty()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 0);
        service.MarkAsPaid(invoice.Id);
        var unpaid = service.GetUnpaidInvoices();
        Assert.Empty(unpaid);
    }

    [Fact]
    public void GetInvoicesByCustomer_WrongCustomerId_ReturnsEmpty()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        service.CreateInvoice(workOrderId, customerId, 0);
        var invoices = service.GetInvoicesByCustomer(999);
        Assert.Empty(invoices);
    }

    [Fact]
    public void GetTotalRevenue_NoInvoices_ReturnsZero()
    {
        var (service, _, _) = CreateServiceWithData();
        Assert.Equal(0m, service.GetTotalRevenue());
    }

    [Fact]
    public void CreateInvoice_WithTenPercentDiscount_LaborCostReduced()
    {
        var (service, workOrderId, customerId) = CreateServiceWithData();
        var invoice = service.CreateInvoice(workOrderId, customerId, 10);
        Assert.Equal(630m, invoice.TotalAmount);
    }
}
