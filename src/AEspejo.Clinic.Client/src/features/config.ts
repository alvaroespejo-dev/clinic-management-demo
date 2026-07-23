import type { ENUM_VALUES } from '@/lib/api/types'

export type EnumName = keyof typeof ENUM_VALUES

export type FieldKind =
  | 'text' | 'number' | 'textarea' | 'checkbox' | 'date' | 'datetime' | 'enum' | 'reference' | 'password'

export interface FieldConfig {
  name: string
  kind: FieldKind
  required?: boolean
  enumName?: EnumName
  /** For FK dropdowns: resource to query and how to label each row. */
  reference?: { resource: string; label: (row: Record<string, unknown>) => string }
  /** 'create' = only shown when creating (e.g. password); 'edit' = only when editing. */
  only?: 'create' | 'edit'
}

export type ColumnKind = 'text' | 'enum' | 'bool' | 'date' | 'datetime' | 'money'

export interface ColumnConfig {
  key: string
  kind?: ColumnKind
  enumName?: EnumName
}

export interface EntityConfig {
  /** Route segment and registry key. */
  key: string
  /** Backend REST resource (e.g. "patients"). */
  resource: string
  /** i18n key for the display name (nav.*). */
  navKey: string
  /** i18n key for the menu group (nav.group*). */
  group: string
  adminOnly?: boolean
  readOnly?: boolean
  idType?: 'string' | 'number'
  columns: ColumnConfig[]
  fields: FieldConfig[]
  /** Default values for the create form. */
  defaults: Record<string, unknown>
}

// Reusable reference-row labels.
export const labelFullName = (r: Record<string, unknown>) => `${r.firstName ?? ''} ${r.lastName ?? ''}`.trim()
export const labelName = (r: Record<string, unknown>) => String(r.name ?? '')
export const labelCode = (r: Record<string, unknown>) => String(r.code ?? r.id ?? '')
