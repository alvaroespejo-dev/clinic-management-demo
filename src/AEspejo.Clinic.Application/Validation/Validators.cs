using AEspejo.Clinic.Application.Auth;
using AEspejo.Clinic.Application.Dtos;
using FluentValidation;

namespace AEspejo.Clinic.Application.Validation;

/// <summary>Empty validator used as a fallback for DTOs without specific rules.</summary>
public class NoOpValidator<T> : AbstractValidator<T> { }

// ---- Auth ----
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

// ---- Branch ----
public class CreateBranchValidator : AbstractValidator<CreateBranchDto>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
public class UpdateBranchValidator : AbstractValidator<UpdateBranchDto>
{
    public UpdateBranchValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
}

// ---- Room ----
public class CreateRoomValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
public class UpdateRoomValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

// ---- User ----
public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role).IsInEnum();
    }
}
public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
    }
}

// ---- Patient ----
public class CreatePatientValidator : AbstractValidator<CreatePatientDto>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.DocumentType).IsInEnum();
    }
}
public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.DocumentNumber).NotEmpty();
    }
}

// ---- Appointment ----
public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProfessionalId).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
    }
}
public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentDto>
{
    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

// ---- ServiceCatalog ----
public class CreateServiceCatalogValidator : AbstractValidator<CreateServiceCatalogDto>
{
    public CreateServiceCatalogValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DefaultPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Translations).NotEmpty().WithMessage("Debe incluir al menos una traducción.");
        RuleForEach(x => x.Translations).ChildRules(t =>
        {
            t.RuleFor(y => y.LanguageCode).NotEmpty().MaximumLength(10);
            t.RuleFor(y => y.Name).NotEmpty().MaximumLength(200);
        });
    }
}
public class UpdateServiceCatalogValidator : AbstractValidator<UpdateServiceCatalogDto>
{
    public UpdateServiceCatalogValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.DefaultPrice).GreaterThanOrEqualTo(0);
    }
}

// ---- Invoice / Payment ----
public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
    }
}
public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).IsInEnum();
    }
}

// ---- PatientAllergy / MedicalCondition / ToothRecord / TreatmentPlan ----
public class CreatePatientAllergyValidator : AbstractValidator<CreatePatientAllergyDto>
{
    public CreatePatientAllergyValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Substance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Severity).IsInEnum();
    }
}
public class UpdatePatientAllergyValidator : AbstractValidator<UpdatePatientAllergyDto>
{
    public UpdatePatientAllergyValidator()
    {
        RuleFor(x => x.Substance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Severity).IsInEnum();
    }
}
public class CreateMedicalConditionValidator : AbstractValidator<CreateMedicalConditionDto>
{
    public CreateMedicalConditionValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
public class UpdateMedicalConditionValidator : AbstractValidator<UpdateMedicalConditionDto>
{
    public UpdateMedicalConditionValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
public class CreateToothRecordValidator : AbstractValidator<CreateToothRecordDto>
{
    public CreateToothRecordValidator()
    {
        RuleFor(x => x.OdontogramId).NotEmpty();
        RuleFor(x => x.ToothNumber).InclusiveBetween(11, 48);
        RuleFor(x => x.Surface).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}
public class UpdateToothRecordValidator : AbstractValidator<UpdateToothRecordDto>
{
    public UpdateToothRecordValidator()
    {
        RuleFor(x => x.ToothNumber).InclusiveBetween(11, 48);
        RuleFor(x => x.Surface).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}
public class CreateTreatmentPlanValidator : AbstractValidator<CreateTreatmentPlanDto>
{
    public CreateTreatmentPlanValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProfessionalId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
public class UpdateTreatmentPlanValidator : AbstractValidator<UpdateTreatmentPlanDto>
{
    public UpdateTreatmentPlanValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
