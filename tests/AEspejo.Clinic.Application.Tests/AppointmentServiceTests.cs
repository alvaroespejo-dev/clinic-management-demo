using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Services;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Domain.Enums;
using AEspejo.Clinic.Infrastructure.Persistence;
using Xunit;

namespace AEspejo.Clinic.Application.Tests;

public class AppointmentServiceTests
{
    private static readonly Guid ProfessionalId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid PatientId = Guid.NewGuid();

    private static AppointmentService NewService(AppDbContext ctx) =>
        new(ctx, TestDb.RepositoryFor<Appointment>(ctx), new FakeCurrentUser());

    private static CreateAppointmentDto NewAppointment(DateTimeOffset at, int minutes) => new()
    {
        BranchId = BranchId,
        PatientId = PatientId,
        ProfessionalId = ProfessionalId,
        ScheduledAt = at,
        DurationMinutes = minutes,
    };

    [Fact]
    public async Task CreateAsync_stamps_status_and_current_user()
    {
        using var ctx = TestDb.NewContext();
        var userId = Guid.NewGuid();
        var service = new AppointmentService(ctx, TestDb.RepositoryFor<Appointment>(ctx), new FakeCurrentUser(userId));

        var result = await service.CreateAsync(NewAppointment(new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero), 30));

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(AppointmentStatus.Scheduled, result.Value!.Status);
        Assert.Equal(userId, result.Value.CreatedByUserId);
        Assert.Single(ctx.Appointments);
    }

    [Fact]
    public async Task CreateAsync_returns_conflict_when_same_professional_overlaps()
    {
        using var ctx = TestDb.NewContext();
        var service = NewService(ctx);
        var start = new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero);

        var first = await service.CreateAsync(NewAppointment(start, 60));
        Assert.Equal(ResultStatus.Ok, first.Status);

        // Starts 30 min into the first appointment → overlaps.
        var second = await service.CreateAsync(NewAppointment(start.AddMinutes(30), 30));

        Assert.Equal(ResultStatus.Conflict, second.Status);
        Assert.Single(ctx.Appointments);
    }

    [Fact]
    public async Task CreateAsync_allows_back_to_back_appointments()
    {
        using var ctx = TestDb.NewContext();
        var service = NewService(ctx);
        var start = new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero);

        await service.CreateAsync(NewAppointment(start, 30));
        // Starts exactly when the first ends → no overlap.
        var second = await service.CreateAsync(NewAppointment(start.AddMinutes(30), 30));

        Assert.Equal(ResultStatus.Ok, second.Status);
        Assert.Equal(2, ctx.Appointments.Count());
    }

    [Fact]
    public async Task CreateAsync_ignores_cancelled_appointments_for_overlap()
    {
        using var ctx = TestDb.NewContext();
        var start = new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero);
        ctx.Appointments.Add(new Appointment
        {
            Id = Guid.NewGuid(),
            BranchId = BranchId,
            PatientId = PatientId,
            ProfessionalId = ProfessionalId,
            ScheduledAt = start,
            DurationMinutes = 60,
            Status = AppointmentStatus.Cancelled,
        });
        await ctx.SaveChangesAsync();

        var service = NewService(ctx);
        var result = await service.CreateAsync(NewAppointment(start, 30));

        Assert.Equal(ResultStatus.Ok, result.Status);
    }
}
