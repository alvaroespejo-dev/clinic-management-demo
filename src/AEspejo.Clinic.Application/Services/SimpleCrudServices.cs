using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

// Entities with soft-delete (IsActive) --------------------------------------

public class BranchService(IAppDbContext db, IRepository<Branch> repo,
    IValidator<CreateBranchDto>? cv = null, IValidator<UpdateBranchDto>? uv = null)
    : SoftDeleteCrudServiceBase<Branch, CreateBranchDto, UpdateBranchDto, BranchDto>(db, repo, cv, uv)
{
    protected override IQueryable<Branch> ApplySearch(IQueryable<Branch> query, string search) =>
        query.Where(b => b.Name.Contains(search) || b.Email.Contains(search));
}

public class RoomService(IAppDbContext db, IRepository<Room> repo,
    IValidator<CreateRoomDto>? cv = null, IValidator<UpdateRoomDto>? uv = null)
    : SoftDeleteCrudServiceBase<Room, CreateRoomDto, UpdateRoomDto, RoomDto>(db, repo, cv, uv)
{
    // Includes the branch so its name can be displayed (Mapster flattens Branch.Name → BranchName).
    protected override IQueryable<Room> BaseQuery() => Repository.Query().Include(r => r.Branch);
}

public class PatientService(IAppDbContext db, IRepository<Patient> repo,
    IValidator<CreatePatientDto>? cv = null, IValidator<UpdatePatientDto>? uv = null)
    : SoftDeleteCrudServiceBase<Patient, CreatePatientDto, UpdatePatientDto, PatientDto>(db, repo, cv, uv)
{
    protected override IQueryable<Patient> ApplySearch(IQueryable<Patient> query, string search) =>
        query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search) || p.DocumentNumber.Contains(search));
}

public class MedicalConditionService(IAppDbContext db, IRepository<MedicalCondition> repo,
    IValidator<CreateMedicalConditionDto>? cv = null, IValidator<UpdateMedicalConditionDto>? uv = null)
    : SoftDeleteCrudServiceBase<MedicalCondition, CreateMedicalConditionDto, UpdateMedicalConditionDto, MedicalConditionDto>(db, repo, cv, uv);

// Entities without soft-delete (physical deletion) --------------------------

public class PatientAllergyService(IAppDbContext db, IRepository<PatientAllergy> repo,
    IValidator<CreatePatientAllergyDto>? cv = null, IValidator<UpdatePatientAllergyDto>? uv = null)
    : CrudServiceBase<PatientAllergy, CreatePatientAllergyDto, UpdatePatientAllergyDto, PatientAllergyDto>(db, repo, cv, uv);

public class OdontogramService(IAppDbContext db, IRepository<Odontogram> repo,
    IValidator<CreateOdontogramDto>? cv = null)
    : CrudServiceBase<Odontogram, CreateOdontogramDto, UpdateOdontogramDto, OdontogramDto>(db, repo, cv);

public class AppointmentDetailService(IAppDbContext db, IRepository<AppointmentDetail> repo,
    IValidator<CreateAppointmentDetailDto>? cv = null, IValidator<UpdateAppointmentDetailDto>? uv = null)
    : CrudServiceBase<AppointmentDetail, CreateAppointmentDetailDto, UpdateAppointmentDetailDto, AppointmentDetailDto>(db, repo, cv, uv);

public class TreatmentPlanService(IAppDbContext db, IRepository<TreatmentPlan> repo,
    IValidator<CreateTreatmentPlanDto>? cv = null, IValidator<UpdateTreatmentPlanDto>? uv = null)
    : CrudServiceBase<TreatmentPlan, CreateTreatmentPlanDto, UpdateTreatmentPlanDto, TreatmentPlanDto>(db, repo, cv, uv);

public class TreatmentPlanItemService(IAppDbContext db, IRepository<TreatmentPlanItem> repo,
    IValidator<CreateTreatmentPlanItemDto>? cv = null, IValidator<UpdateTreatmentPlanItemDto>? uv = null)
    : CrudServiceBase<TreatmentPlanItem, CreateTreatmentPlanItemDto, UpdateTreatmentPlanItemDto, TreatmentPlanItemDto>(db, repo, cv, uv);

public class InvoiceService(IAppDbContext db, IRepository<Invoice> repo,
    IValidator<CreateInvoiceDto>? cv = null, IValidator<UpdateInvoiceDto>? uv = null)
    : CrudServiceBase<Invoice, CreateInvoiceDto, UpdateInvoiceDto, InvoiceDto>(db, repo, cv, uv);
