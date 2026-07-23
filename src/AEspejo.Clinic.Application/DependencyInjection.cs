using AEspejo.Clinic.Application.Auth;
using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Services;
using AEspejo.Clinic.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AEspejo.Clinic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Mapster mappings not covered by the automatic flattening convention (see MapsterConfig).
        MapsterConfig.RegisterMappings();

        // Validators: no-op fallback (open generic) first; specific ones win by being registered afterwards.
        services.AddScoped(typeof(IValidator<>), typeof(NoOpValidator<>));
        services.AddValidatorsFromAssemblyContaining<NoOpValidator<object>>(ServiceLifetime.Scoped);

        // Standard CRUD services (injectable as ICrudService<,,>).
        services.AddScoped<ICrudService<CreateBranchDto, UpdateBranchDto, BranchDto>, BranchService>();
        services.AddScoped<ICrudService<CreateRoomDto, UpdateRoomDto, RoomDto>, RoomService>();
        services.AddScoped<ICrudService<CreateUserDto, UpdateUserDto, UserDto>, UserService>();
        services.AddScoped<ICrudService<CreateProfessionalDto, UpdateProfessionalDto, ProfessionalDto>, ProfessionalService>();
        services.AddScoped<ICrudService<CreatePatientDto, UpdatePatientDto, PatientDto>, PatientService>();
        services.AddScoped<ICrudService<CreatePatientAllergyDto, UpdatePatientAllergyDto, PatientAllergyDto>, PatientAllergyService>();
        services.AddScoped<ICrudService<CreateMedicalConditionDto, UpdateMedicalConditionDto, MedicalConditionDto>, MedicalConditionService>();
        services.AddScoped<ICrudService<CreateOdontogramDto, UpdateOdontogramDto, OdontogramDto>, OdontogramService>();
        services.AddScoped<ICrudService<CreateToothRecordDto, UpdateToothRecordDto, ToothRecordDto>, ToothRecordService>();
        services.AddScoped<ICrudService<CreateAppointmentDto, UpdateAppointmentDto, AppointmentDto>, AppointmentService>();
        services.AddScoped<ICrudService<CreateAppointmentDetailDto, UpdateAppointmentDetailDto, AppointmentDetailDto>, AppointmentDetailService>();
        services.AddScoped<ICrudService<CreateTreatmentPlanDto, UpdateTreatmentPlanDto, TreatmentPlanDto>, TreatmentPlanService>();
        services.AddScoped<ICrudService<CreateTreatmentPlanItemDto, UpdateTreatmentPlanItemDto, TreatmentPlanItemDto>, TreatmentPlanItemService>();
        services.AddScoped<ICrudService<CreateInvoiceDto, UpdateInvoiceDto, InvoiceDto>, InvoiceService>();
        services.AddScoped<ICrudService<CreatePaymentDto, UpdatePaymentDto, PaymentDto>, PaymentService>();

        // Services with their own contract.
        services.AddScoped<ConfigService>();
        services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
