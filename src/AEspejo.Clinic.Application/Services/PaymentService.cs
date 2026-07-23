using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Domain.Enums;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

/// <summary>
/// Records ReceivedByUserId with the request's user and recalculates the invoice's
/// PaidAmount/Status after every payment create/update/delete.
/// </summary>
public class PaymentService(IAppDbContext db, IRepository<Payment> repo, ICurrentUserService currentUser,
    IValidator<CreatePaymentDto>? cv = null, IValidator<UpdatePaymentDto>? uv = null)
    : CrudServiceBase<Payment, CreatePaymentDto, UpdatePaymentDto, PaymentDto>(db, repo, cv, uv)
{

    public override async Task<Result<PaymentDto>> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var invoice = await Db.Invoices.FirstOrDefaultAsync(i => i.Id == dto.InvoiceId, ct);
        if (invoice is null) return Result<PaymentDto>.Invalid("La factura indicada no existe.");

        var entity = dto.Adapt<Payment>();
        entity.ReceivedByUserId = currentUser.UserId ?? Guid.Empty;
        await Repository.AddAsync(entity, ct);

        await RecalculateInvoiceAsync(dto.InvoiceId, ct);
        return Result<PaymentDto>.Ok(entity.Adapt<PaymentDto>());
    }

    public override async Task<Result<PaymentDto>> UpdateAsync(Guid id, UpdatePaymentDto dto, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return Result<PaymentDto>.NotFound();

        dto.Adapt(entity);
        await Repository.UpdateAsync(entity, ct);
        await RecalculateInvoiceAsync(entity.InvoiceId, ct);
        return Result<PaymentDto>.Ok(entity.Adapt<PaymentDto>());
    }

    public override async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return Result.NotFound();

        var invoiceId = entity.InvoiceId;
        await Repository.DeleteAsync(entity, ct);
        await RecalculateInvoiceAsync(invoiceId, ct);
        return Result.Ok();
    }

    private async Task RecalculateInvoiceAsync(Guid invoiceId, CancellationToken ct)
    {
        var invoice = await Db.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId, ct);
        if (invoice is null) return;

        var paid = await Db.Payments.Where(p => p.InvoiceId == invoiceId).SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;
        invoice.PaidAmount = paid;

        if (invoice.Status != InvoiceStatus.Cancelled && invoice.Status != InvoiceStatus.Draft)
        {
            invoice.Status = paid <= 0 ? InvoiceStatus.Issued
                : paid >= invoice.TotalAmount ? InvoiceStatus.Paid
                : InvoiceStatus.PartiallyPaid;
        }

        await Db.SaveChangesAsync(ct);
    }
}
