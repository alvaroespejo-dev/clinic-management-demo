using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Services;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Domain.Enums;
using AEspejo.Clinic.Infrastructure.Persistence;
using Xunit;

namespace AEspejo.Clinic.Application.Tests;

public class PaymentServiceTests
{
    private static PaymentService NewService(AppDbContext ctx) =>
        new(ctx, TestDb.RepositoryFor<Payment>(ctx), new FakeCurrentUser());

    private static async Task<Invoice> SeedInvoiceAsync(AppDbContext ctx, decimal total, InvoiceStatus status)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            InvoiceNumber = "INV-001",
            IssueDate = new DateOnly(2026, 7, 22),
            TotalAmount = total,
            Status = status,
        };
        ctx.Invoices.Add(invoice);
        await ctx.SaveChangesAsync();
        return invoice;
    }

    private static CreatePaymentDto NewPayment(Guid invoiceId, decimal amount) => new()
    {
        InvoiceId = invoiceId,
        Amount = amount,
        Method = PaymentMethod.Cash,
        PaidAt = new DateTimeOffset(2026, 7, 22, 12, 0, 0, TimeSpan.Zero),
    };

    [Fact]
    public async Task CreateAsync_returns_invalid_when_invoice_missing()
    {
        using var ctx = TestDb.NewContext();
        var result = await NewService(ctx).CreateAsync(NewPayment(Guid.NewGuid(), 50m));

        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Empty(ctx.Payments);
    }

    [Fact]
    public async Task CreateAsync_partial_payment_sets_status_partially_paid()
    {
        using var ctx = TestDb.NewContext();
        var invoice = await SeedInvoiceAsync(ctx, total: 100m, status: InvoiceStatus.Issued);

        var result = await NewService(ctx).CreateAsync(NewPayment(invoice.Id, 40m));

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(40m, invoice.PaidAmount);
        Assert.Equal(InvoiceStatus.PartiallyPaid, invoice.Status);
    }

    [Fact]
    public async Task CreateAsync_full_payment_sets_status_paid()
    {
        using var ctx = TestDb.NewContext();
        var invoice = await SeedInvoiceAsync(ctx, total: 100m, status: InvoiceStatus.Issued);
        var service = NewService(ctx);

        await service.CreateAsync(NewPayment(invoice.Id, 40m));
        await service.CreateAsync(NewPayment(invoice.Id, 60m));

        Assert.Equal(100m, invoice.PaidAmount);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public async Task DeleteAsync_recalculates_invoice_back_down()
    {
        using var ctx = TestDb.NewContext();
        var invoice = await SeedInvoiceAsync(ctx, total: 100m, status: InvoiceStatus.Issued);
        var service = NewService(ctx);

        var created = await service.CreateAsync(NewPayment(invoice.Id, 100m));
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);

        var delete = await service.DeleteAsync(created.Value!.Id);

        Assert.Equal(ResultStatus.Ok, delete.Status);
        Assert.Equal(0m, invoice.PaidAmount);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
    }
}
