using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

// ---- Organization / staff ----

[Route("api/branches")]
[Authorize(Roles = "Admin")]
public class BranchesController(ICrudService<CreateBranchDto, UpdateBranchDto, BranchDto> s)
    : CrudControllerBase<CreateBranchDto, UpdateBranchDto, BranchDto>(s);

[Route("api/rooms")]
public class RoomsController(ICrudService<CreateRoomDto, UpdateRoomDto, RoomDto> s)
    : CrudControllerBase<CreateRoomDto, UpdateRoomDto, RoomDto>(s);

[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController(ICrudService<CreateUserDto, UpdateUserDto, UserDto> s)
    : CrudControllerBase<CreateUserDto, UpdateUserDto, UserDto>(s);

[Route("api/professionals")]
public class ProfessionalsController(ICrudService<CreateProfessionalDto, UpdateProfessionalDto, ProfessionalDto> s)
    : CrudControllerBase<CreateProfessionalDto, UpdateProfessionalDto, ProfessionalDto>(s);

// ---- Patients ----

[Route("api/patients")]
public class PatientsController(ICrudService<CreatePatientDto, UpdatePatientDto, PatientDto> s)
    : CrudControllerBase<CreatePatientDto, UpdatePatientDto, PatientDto>(s);

[Route("api/patient-allergies")]
public class PatientAllergiesController(ICrudService<CreatePatientAllergyDto, UpdatePatientAllergyDto, PatientAllergyDto> s)
    : CrudControllerBase<CreatePatientAllergyDto, UpdatePatientAllergyDto, PatientAllergyDto>(s);

[Route("api/medical-conditions")]
public class MedicalConditionsController(ICrudService<CreateMedicalConditionDto, UpdateMedicalConditionDto, MedicalConditionDto> s)
    : CrudControllerBase<CreateMedicalConditionDto, UpdateMedicalConditionDto, MedicalConditionDto>(s);

[Route("api/odontograms")]
public class OdontogramsController(ICrudService<CreateOdontogramDto, UpdateOdontogramDto, OdontogramDto> s)
    : CrudControllerBase<CreateOdontogramDto, UpdateOdontogramDto, OdontogramDto>(s);

[Route("api/tooth-records")]
public class ToothRecordsController(ICrudService<CreateToothRecordDto, UpdateToothRecordDto, ToothRecordDto> s)
    : CrudControllerBase<CreateToothRecordDto, UpdateToothRecordDto, ToothRecordDto>(s);

// ---- Appointments / treatments ----

[Route("api/appointments")]
public class AppointmentsController(ICrudService<CreateAppointmentDto, UpdateAppointmentDto, AppointmentDto> s)
    : CrudControllerBase<CreateAppointmentDto, UpdateAppointmentDto, AppointmentDto>(s);

[Route("api/appointment-details")]
public class AppointmentDetailsController(ICrudService<CreateAppointmentDetailDto, UpdateAppointmentDetailDto, AppointmentDetailDto> s)
    : CrudControllerBase<CreateAppointmentDetailDto, UpdateAppointmentDetailDto, AppointmentDetailDto>(s);

[Route("api/treatment-plans")]
public class TreatmentPlansController(ICrudService<CreateTreatmentPlanDto, UpdateTreatmentPlanDto, TreatmentPlanDto> s)
    : CrudControllerBase<CreateTreatmentPlanDto, UpdateTreatmentPlanDto, TreatmentPlanDto>(s);

[Route("api/treatment-plan-items")]
public class TreatmentPlanItemsController(ICrudService<CreateTreatmentPlanItemDto, UpdateTreatmentPlanItemDto, TreatmentPlanItemDto> s)
    : CrudControllerBase<CreateTreatmentPlanItemDto, UpdateTreatmentPlanItemDto, TreatmentPlanItemDto>(s);

// ---- Billing ----

[Route("api/invoices")]
public class InvoicesController(ICrudService<CreateInvoiceDto, UpdateInvoiceDto, InvoiceDto> s)
    : CrudControllerBase<CreateInvoiceDto, UpdateInvoiceDto, InvoiceDto>(s);

[Route("api/payments")]
public class PaymentsController(ICrudService<CreatePaymentDto, UpdatePaymentDto, PaymentDto> s)
    : CrudControllerBase<CreatePaymentDto, UpdatePaymentDto, PaymentDto>(s);
