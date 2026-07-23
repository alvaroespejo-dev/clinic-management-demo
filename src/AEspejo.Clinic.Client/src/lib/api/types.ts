// Types mirroring the .NET backend DTOs. API enums are serialized as
// strings, so they are string unions here (the value is the i18n key, e.g. gender.Male).
// The `npm run gen:api` pipeline generates schema.d.ts from OpenAPI to check that there is no drift.

// ---- Enums (string unions) ----
export type UserRole = 'Admin' | 'Dentist' | 'Assistant' | 'Receptionist'
export type Gender = 'Male' | 'Female' | 'Other'
export type DocumentType = 'NationalId' | 'Passport' | 'Other'
export type AllergySeverity = 'Mild' | 'Moderate' | 'Severe'
export type ToothSurface = 'Buccal' | 'Lingual' | 'Mesial' | 'Distal' | 'Occlusal' | 'Full'
export type ToothStatus =
  | 'Healthy' | 'Caries' | 'Filled' | 'Extracted' | 'Crown' | 'Implant' | 'RootCanal' | 'Missing'
export type AppointmentStatus =
  | 'Scheduled' | 'Confirmed' | 'InProgress' | 'Completed' | 'Cancelled' | 'NoShow'
export type TreatmentPlanStatus = 'Draft' | 'Active' | 'Completed' | 'Cancelled'
export type TreatmentPlanItemStatus = 'Pending' | 'InProgress' | 'Completed' | 'Skipped'
export type InvoiceStatus = 'Draft' | 'Issued' | 'PartiallyPaid' | 'Paid' | 'Cancelled'
export type PaymentMethod = 'Cash' | 'Card' | 'Transfer' | 'Insurance'
export type AuditAction = 'Created' | 'Updated' | 'Deleted'

export const ENUM_VALUES = {
  UserRole: ['Admin', 'Dentist', 'Assistant', 'Receptionist'] as const,
  Gender: ['Male', 'Female', 'Other'] as const,
  DocumentType: ['NationalId', 'Passport', 'Other'] as const,
  AllergySeverity: ['Mild', 'Moderate', 'Severe'] as const,
  ToothSurface: ['Buccal', 'Lingual', 'Mesial', 'Distal', 'Occlusal', 'Full'] as const,
  ToothStatus: ['Healthy', 'Caries', 'Filled', 'Extracted', 'Crown', 'Implant', 'RootCanal', 'Missing'] as const,
  AppointmentStatus: ['Scheduled', 'Confirmed', 'InProgress', 'Completed', 'Cancelled', 'NoShow'] as const,
  TreatmentPlanStatus: ['Draft', 'Active', 'Completed', 'Cancelled'] as const,
  TreatmentPlanItemStatus: ['Pending', 'InProgress', 'Completed', 'Skipped'] as const,
  InvoiceStatus: ['Draft', 'Issued', 'PartiallyPaid', 'Paid', 'Cancelled'] as const,
  PaymentMethod: ['Cash', 'Card', 'Transfer', 'Insurance'] as const,
  AuditAction: ['Created', 'Updated', 'Deleted'] as const,
}

// ---- Pagination ----
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

export interface PagedQuery {
  page?: number
  pageSize?: number
  search?: string
  includeInactive?: boolean
}

// ---- Auth ----
export interface LoginRequest { email: string; password: string }
export interface LoginResponse {
  token: string
  expiresAt: string
  userId: string
  fullName: string
  role: UserRole
  preferredLanguage: string
}

// ---- Entities (response + create/update) ----
export interface Branch { id: string; name: string; address: string; phone: string; email: string; isActive: boolean; createdAt: string }
export interface BranchInput { name: string; address: string; phone: string; email: string; isActive?: boolean }

export interface Room { id: string; branchId: string; branchName: string; name: string; isActive: boolean; createdAt: string }
export interface RoomCreate { branchId: string; name: string }
export interface RoomUpdate { name: string; isActive: boolean }

export interface User { id: string; email: string; firstName: string; lastName: string; role: UserRole; preferredLanguage: string; branchId?: string | null; isActive: boolean; createdAt: string }
export interface UserCreate { email: string; password: string; firstName: string; lastName: string; role: UserRole; preferredLanguage: string; branchId?: string | null }
export interface UserUpdate { firstName: string; lastName: string; role: UserRole; preferredLanguage: string; branchId?: string | null; isActive: boolean }

export interface Professional { id: string; licenseNumber: string; specialty: string; color: string }
export interface ProfessionalCreate { userId: string; licenseNumber: string; specialty: string; color: string }
export interface ProfessionalUpdate { licenseNumber: string; specialty: string; color: string }

export interface Patient {
  id: string; branchId: string; firstName: string; lastName: string; dateOfBirth: string
  gender: Gender; documentType: DocumentType; documentNumber: string; phone: string; email: string
  address: string; bloodType?: string | null; preferredLanguage: string
  emergencyContactName?: string | null; emergencyContactPhone?: string | null; notes?: string | null
  isActive: boolean; createdAt: string
}
export interface PatientCreate {
  branchId: string; firstName: string; lastName: string; dateOfBirth: string
  gender: Gender; documentType: DocumentType; documentNumber: string; phone: string; email: string
  address: string; bloodType?: string | null; preferredLanguage: string
  emergencyContactName?: string | null; emergencyContactPhone?: string | null; notes?: string | null
}
export interface PatientUpdate extends PatientCreate { isActive: boolean }

export interface PatientAllergy { id: string; patientId: string; substance: string; severity: AllergySeverity; notes?: string | null }
export interface PatientAllergyCreate { patientId: string; substance: string; severity: AllergySeverity; notes?: string | null }
export interface PatientAllergyUpdate { substance: string; severity: AllergySeverity; notes?: string | null }

export interface MedicalCondition { id: string; patientId: string; name: string; notes?: string | null; isActive: boolean }
export interface MedicalConditionCreate { patientId: string; name: string; notes?: string | null }
export interface MedicalConditionUpdate { name: string; notes?: string | null; isActive: boolean }

export interface Odontogram { id: string; patientId: string; createdAt: string }
export interface OdontogramCreate { patientId: string }

export interface ToothRecord { id: string; odontogramId: string; toothNumber: number; surface: ToothSurface; status: ToothStatus; notes?: string | null; updatedByUserId: string; updatedAt?: string | null }
export interface ToothRecordCreate { odontogramId: string; toothNumber: number; surface: ToothSurface; status: ToothStatus; notes?: string | null }
export interface ToothRecordUpdate { toothNumber: number; surface: ToothSurface; status: ToothStatus; notes?: string | null }

export interface ServiceTranslation { languageCode: string; name: string; description?: string | null; category: string }
export interface ServiceCatalog { id: string; code: string; defaultPrice: number; isActive: boolean; name: string; description?: string | null; category: string; translations: ServiceTranslation[] }
export interface ServiceCatalogCreate { code: string; defaultPrice: number; translations: ServiceTranslation[] }
export interface ServiceCatalogUpdate { code: string; defaultPrice: number; isActive: boolean; translations: ServiceTranslation[] }

export interface Appointment { id: string; branchId: string; patientId: string; professionalId: string; roomId?: string | null; scheduledAt: string; durationMinutes: number; status: AppointmentStatus; notes?: string | null; cancelReason?: string | null; createdByUserId: string; createdAt: string }
export interface AppointmentCreate { branchId: string; patientId: string; professionalId: string; roomId?: string | null; scheduledAt: string; durationMinutes: number; notes?: string | null }
export interface AppointmentUpdate { roomId?: string | null; scheduledAt: string; durationMinutes: number; status: AppointmentStatus; notes?: string | null; cancelReason?: string | null }

export interface AppointmentDetail { id: string; appointmentId: string; serviceId: string; toothNumber?: number | null; quantity: number; unitPrice: number; notes?: string | null }
export interface AppointmentDetailCreate { appointmentId: string; serviceId: string; toothNumber?: number | null; quantity: number; unitPrice: number; notes?: string | null }
export interface AppointmentDetailUpdate { toothNumber?: number | null; quantity: number; unitPrice: number; notes?: string | null }

export interface TreatmentPlan { id: string; patientId: string; professionalId: string; title: string; description?: string | null; status: TreatmentPlanStatus; createdAt: string }
export interface TreatmentPlanCreate { patientId: string; professionalId: string; title: string; description?: string | null }
export interface TreatmentPlanUpdate { title: string; description?: string | null; status: TreatmentPlanStatus }

export interface TreatmentPlanItem { id: string; treatmentPlanId: string; serviceId: string; toothNumber?: number | null; quantity: number; estimatedPrice: number; status: TreatmentPlanItemStatus; appointmentDetailId?: string | null }
export interface TreatmentPlanItemCreate { treatmentPlanId: string; serviceId: string; toothNumber?: number | null; quantity: number; estimatedPrice: number }
export interface TreatmentPlanItemUpdate { toothNumber?: number | null; quantity: number; estimatedPrice: number; status: TreatmentPlanItemStatus; appointmentDetailId?: string | null }

export interface Invoice { id: string; branchId: string; patientId: string; appointmentId?: string | null; invoiceNumber: string; issueDate: string; totalAmount: number; paidAmount: number; status: InvoiceStatus; notes?: string | null; createdAt: string }
export interface InvoiceCreate { branchId: string; patientId: string; appointmentId?: string | null; invoiceNumber: string; issueDate: string; totalAmount: number; notes?: string | null }
export interface InvoiceUpdate { invoiceNumber: string; issueDate: string; totalAmount: number; status: InvoiceStatus; notes?: string | null }

export interface Payment { id: string; invoiceId: string; amount: number; method: PaymentMethod; paidAt: string; reference?: string | null; receivedByUserId: string }
export interface PaymentCreate { invoiceId: string; amount: number; method: PaymentMethod; paidAt: string; reference?: string | null }
export interface PaymentUpdate { amount: number; method: PaymentMethod; paidAt: string; reference?: string | null }

export interface AuditLog { id: number; tableName: string; recordId: string; action: AuditAction; oldValues?: string | null; newValues?: string | null; userId?: string | null; changedAt: string; ipAddress?: string | null }
