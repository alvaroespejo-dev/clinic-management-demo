import type { EntityConfig } from './config'
import { labelFullName, labelName, labelCode } from './config'

// Central registry of the 18 entities. The menu and routes are generated from here.
export const ENTITIES: EntityConfig[] = [
  // ---- Organization ----
  {
    key: 'branches', resource: 'branches', navKey: 'nav.branches', group: 'nav.groupOrg', adminOnly: true,
    columns: [{ key: 'name' }, { key: 'phone' }, { key: 'email' }, { key: 'isActive', kind: 'bool' }],
    fields: [
      { name: 'name', kind: 'text', required: true }, { name: 'address', kind: 'text' },
      { name: 'phone', kind: 'text' }, { name: 'email', kind: 'text' }, { name: 'isActive', kind: 'checkbox', only: 'edit' },
    ],
    defaults: { name: '', address: '', phone: '', email: '', isActive: true },
  },
  {
    key: 'rooms', resource: 'rooms', navKey: 'nav.rooms', group: 'nav.groupOrg',
    columns: [{ key: 'name' }, { key: 'branchName' }, { key: 'isActive', kind: 'bool' }],
    fields: [
      { name: 'branchId', kind: 'reference', required: true, reference: { resource: 'branches', label: labelName } },
      { name: 'name', kind: 'text', required: true }, { name: 'isActive', kind: 'checkbox', only: 'edit' },
    ],
    defaults: { branchId: '', name: '', isActive: true },
  },
  {
    key: 'users', resource: 'users', navKey: 'nav.users', group: 'nav.groupOrg', adminOnly: true,
    columns: [{ key: 'firstName' }, { key: 'lastName' }, { key: 'email' }, { key: 'role', kind: 'enum', enumName: 'UserRole' }, { key: 'isActive', kind: 'bool' }],
    fields: [
      { name: 'email', kind: 'text', required: true, only: 'create' },
      { name: 'password', kind: 'password', required: true, only: 'create' },
      { name: 'firstName', kind: 'text', required: true }, { name: 'lastName', kind: 'text', required: true },
      { name: 'role', kind: 'enum', enumName: 'UserRole', required: true },
      { name: 'preferredLanguage', kind: 'text' },
      { name: 'branchId', kind: 'reference', reference: { resource: 'branches', label: labelName } },
      { name: 'isActive', kind: 'checkbox', only: 'edit' },
    ],
    defaults: { email: '', password: '', firstName: '', lastName: '', role: 'Receptionist', preferredLanguage: 'es', branchId: '', isActive: true },
  },
  {
    key: 'professionals', resource: 'professionals', navKey: 'nav.professionals', group: 'nav.groupOrg',
    columns: [{ key: 'firstName' }, { key: 'lastName' }, { key: 'licenseNumber' }, { key: 'specialty' }],
    fields: [
      { name: 'userId', kind: 'reference', required: true, only: 'create', reference: { resource: 'users', label: labelFullName } },
      { name: 'licenseNumber', kind: 'text' }, { name: 'specialty', kind: 'text' }, { name: 'color', kind: 'text' },
    ],
    defaults: { userId: '', licenseNumber: '', specialty: '', color: '#3b82f6' },
  },

  // ---- Patients ----
  {
    key: 'patients', resource: 'patients', navKey: 'nav.patients', group: 'nav.groupPatients',
    columns: [{ key: 'firstName' }, { key: 'lastName' }, { key: 'documentNumber' }, { key: 'phone' }, { key: 'gender', kind: 'enum', enumName: 'Gender' }, { key: 'isActive', kind: 'bool' }],
    fields: [
      { name: 'branchId', kind: 'reference', required: true, reference: { resource: 'branches', label: labelName } },
      { name: 'firstName', kind: 'text', required: true }, { name: 'lastName', kind: 'text', required: true },
      { name: 'dateOfBirth', kind: 'date', required: true },
      { name: 'gender', kind: 'enum', enumName: 'Gender', required: true },
      { name: 'documentType', kind: 'enum', enumName: 'DocumentType', required: true },
      { name: 'documentNumber', kind: 'text', required: true },
      { name: 'phone', kind: 'text' }, { name: 'email', kind: 'text' }, { name: 'address', kind: 'text' },
      { name: 'bloodType', kind: 'text' }, { name: 'preferredLanguage', kind: 'text' },
      { name: 'emergencyContactName', kind: 'text' }, { name: 'emergencyContactPhone', kind: 'text' },
      { name: 'notes', kind: 'textarea' }, { name: 'isActive', kind: 'checkbox', only: 'edit' },
    ],
    defaults: {
      branchId: '', firstName: '', lastName: '', dateOfBirth: '', gender: 'Male', documentType: 'NationalId',
      documentNumber: '', phone: '', email: '', address: '', bloodType: '', preferredLanguage: 'es',
      emergencyContactName: '', emergencyContactPhone: '', notes: '', isActive: true,
    },
  },
  {
    key: 'patient-allergies', resource: 'patient-allergies', navKey: 'nav.patientAllergies', group: 'nav.groupPatients',
    columns: [{ key: 'substance' }, { key: 'severity', kind: 'enum', enumName: 'AllergySeverity' }],
    fields: [
      { name: 'patientId', kind: 'reference', required: true, reference: { resource: 'patients', label: labelFullName } },
      { name: 'substance', kind: 'text', required: true },
      { name: 'severity', kind: 'enum', enumName: 'AllergySeverity', required: true }, { name: 'notes', kind: 'textarea' },
    ],
    defaults: { patientId: '', substance: '', severity: 'Mild', notes: '' },
  },
  {
    key: 'medical-conditions', resource: 'medical-conditions', navKey: 'nav.medicalConditions', group: 'nav.groupPatients',
    columns: [{ key: 'name' }, { key: 'isActive', kind: 'bool' }],
    fields: [
      { name: 'patientId', kind: 'reference', required: true, reference: { resource: 'patients', label: labelFullName } },
      { name: 'name', kind: 'text', required: true }, { name: 'notes', kind: 'textarea' }, { name: 'isActive', kind: 'checkbox', only: 'edit' },
    ],
    defaults: { patientId: '', name: '', notes: '', isActive: true },
  },
  {
    key: 'odontograms', resource: 'odontograms', navKey: 'nav.odontograms', group: 'nav.groupPatients',
    columns: [{ key: 'patientId' }, { key: 'createdAt', kind: 'datetime' }],
    fields: [{ name: 'patientId', kind: 'reference', required: true, only: 'create', reference: { resource: 'patients', label: labelFullName } }],
    defaults: { patientId: '' },
  },
  {
    key: 'tooth-records', resource: 'tooth-records', navKey: 'nav.toothRecords', group: 'nav.groupPatients',
    columns: [{ key: 'toothNumber' }, { key: 'surface', kind: 'enum', enumName: 'ToothSurface' }, { key: 'status', kind: 'enum', enumName: 'ToothStatus' }],
    fields: [
      { name: 'odontogramId', kind: 'reference', required: true, reference: { resource: 'odontograms', label: (r) => String(r.id).slice(0, 8) } },
      { name: 'toothNumber', kind: 'number', required: true },
      { name: 'surface', kind: 'enum', enumName: 'ToothSurface', required: true },
      { name: 'status', kind: 'enum', enumName: 'ToothStatus', required: true }, { name: 'notes', kind: 'textarea' },
    ],
    defaults: { odontogramId: '', toothNumber: 11, surface: 'Full', status: 'Healthy', notes: '' },
  },

  // ---- Clinical ----
  {
    key: 'appointments', resource: 'appointments', navKey: 'nav.appointments', group: 'nav.groupClinical',
    columns: [{ key: 'scheduledAt', kind: 'datetime' }, { key: 'durationMinutes' }, { key: 'status', kind: 'enum', enumName: 'AppointmentStatus' }],
    fields: [
      { name: 'branchId', kind: 'reference', required: true, reference: { resource: 'branches', label: labelName } },
      { name: 'patientId', kind: 'reference', required: true, reference: { resource: 'patients', label: labelFullName } },
      { name: 'professionalId', kind: 'reference', required: true, reference: { resource: 'users', label: labelFullName } },
      { name: 'roomId', kind: 'reference', reference: { resource: 'rooms', label: labelName } },
      { name: 'scheduledAt', kind: 'datetime', required: true },
      { name: 'durationMinutes', kind: 'number', required: true },
      { name: 'status', kind: 'enum', enumName: 'AppointmentStatus', only: 'edit' },
      { name: 'notes', kind: 'textarea' }, { name: 'cancelReason', kind: 'text', only: 'edit' },
    ],
    defaults: { branchId: '', patientId: '', professionalId: '', roomId: '', scheduledAt: '', durationMinutes: 30, status: 'Scheduled', notes: '', cancelReason: '' },
  },
  {
    key: 'appointment-details', resource: 'appointment-details', navKey: 'nav.appointmentDetails', group: 'nav.groupClinical',
    columns: [{ key: 'quantity' }, { key: 'unitPrice', kind: 'money' }],
    fields: [
      { name: 'appointmentId', kind: 'reference', required: true, reference: { resource: 'appointments', label: (r) => String(r.id).slice(0, 8) } },
      { name: 'serviceId', kind: 'reference', required: true, reference: { resource: 'services', label: labelCode } },
      { name: 'toothNumber', kind: 'number' }, { name: 'quantity', kind: 'number', required: true },
      { name: 'unitPrice', kind: 'number', required: true }, { name: 'notes', kind: 'textarea' },
    ],
    defaults: { appointmentId: '', serviceId: '', toothNumber: null, quantity: 1, unitPrice: 0, notes: '' },
  },
  {
    key: 'treatment-plans', resource: 'treatment-plans', navKey: 'nav.treatmentPlans', group: 'nav.groupClinical',
    columns: [{ key: 'title' }, { key: 'status', kind: 'enum', enumName: 'TreatmentPlanStatus' }],
    fields: [
      { name: 'patientId', kind: 'reference', required: true, reference: { resource: 'patients', label: labelFullName } },
      { name: 'professionalId', kind: 'reference', required: true, reference: { resource: 'users', label: labelFullName } },
      { name: 'title', kind: 'text', required: true }, { name: 'description', kind: 'textarea' },
      { name: 'status', kind: 'enum', enumName: 'TreatmentPlanStatus', only: 'edit' },
    ],
    defaults: { patientId: '', professionalId: '', title: '', description: '', status: 'Draft' },
  },
  {
    key: 'treatment-plan-items', resource: 'treatment-plan-items', navKey: 'nav.treatmentPlanItems', group: 'nav.groupClinical',
    columns: [{ key: 'quantity' }, { key: 'estimatedPrice', kind: 'money' }, { key: 'status', kind: 'enum', enumName: 'TreatmentPlanItemStatus' }],
    fields: [
      { name: 'treatmentPlanId', kind: 'reference', required: true, reference: { resource: 'treatment-plans', label: (r) => String(r.title ?? r.id) } },
      { name: 'serviceId', kind: 'reference', required: true, reference: { resource: 'services', label: labelCode } },
      { name: 'toothNumber', kind: 'number' }, { name: 'quantity', kind: 'number', required: true },
      { name: 'estimatedPrice', kind: 'number', required: true },
      { name: 'status', kind: 'enum', enumName: 'TreatmentPlanItemStatus', only: 'edit' },
    ],
    defaults: { treatmentPlanId: '', serviceId: '', toothNumber: null, quantity: 1, estimatedPrice: 0, status: 'Pending' },
  },

  // ---- Billing ----
  {
    key: 'invoices', resource: 'invoices', navKey: 'nav.invoices', group: 'nav.groupBilling',
    columns: [{ key: 'invoiceNumber' }, { key: 'totalAmount', kind: 'money' }, { key: 'paidAmount', kind: 'money' }, { key: 'status', kind: 'enum', enumName: 'InvoiceStatus' }],
    fields: [
      { name: 'branchId', kind: 'reference', required: true, reference: { resource: 'branches', label: labelName } },
      { name: 'patientId', kind: 'reference', required: true, reference: { resource: 'patients', label: labelFullName } },
      { name: 'invoiceNumber', kind: 'text', required: true }, { name: 'issueDate', kind: 'date', required: true },
      { name: 'totalAmount', kind: 'number', required: true },
      { name: 'status', kind: 'enum', enumName: 'InvoiceStatus', only: 'edit' }, { name: 'notes', kind: 'textarea' },
    ],
    defaults: { branchId: '', patientId: '', invoiceNumber: '', issueDate: '', totalAmount: 0, status: 'Draft', notes: '' },
  },
  {
    key: 'payments', resource: 'payments', navKey: 'nav.payments', group: 'nav.groupBilling',
    columns: [{ key: 'amount', kind: 'money' }, { key: 'method', kind: 'enum', enumName: 'PaymentMethod' }, { key: 'paidAt', kind: 'datetime' }],
    fields: [
      { name: 'invoiceId', kind: 'reference', required: true, reference: { resource: 'invoices', label: (r) => String(r.invoiceNumber ?? r.id) } },
      { name: 'amount', kind: 'number', required: true },
      { name: 'method', kind: 'enum', enumName: 'PaymentMethod', required: true },
      { name: 'paidAt', kind: 'datetime', required: true }, { name: 'reference', kind: 'text' },
    ],
    defaults: { invoiceId: '', amount: 0, method: 'Cash', paidAt: '', reference: '' },
  },

  // ---- System ----
  {
    key: 'audit-logs', resource: 'audit-logs', navKey: 'nav.auditLogs', group: 'nav.groupSystem', adminOnly: true, readOnly: true, idType: 'number',
    columns: [{ key: 'tableName' }, { key: 'recordId' }, { key: 'action', kind: 'enum', enumName: 'AuditAction' }, { key: 'changedAt', kind: 'datetime' }],
    fields: [],
    defaults: {},
  },
]

export const ENTITY_MAP: Record<string, EntityConfig> = Object.fromEntries(ENTITIES.map((e) => [e.key, e]))
