import * as React from 'react'
import { useTranslation } from 'react-i18next'
import type { TFunction } from 'i18next'
import { Pencil, Trash2, Plus, Inbox } from 'lucide-react'
import type { EntityConfig, ColumnConfig } from '@/features/config'
import { createCrud } from '@/lib/api/crud'
import { apiErrorMessage } from '@/lib/api/client'
import { useAuth } from '@/lib/auth/AuthContext'
import { useToast } from '@/lib/toast/ToastContext'
import { useConfirmDialog } from '@/lib/dialog/useConfirmDialog'
import { useKeyboardShortcuts } from '@/lib/hooks/useKeyboardShortcuts'
import { Button, Input, Card } from '@/components/ui/primitives'
import { Dialog } from '@/components/ui/dialog'
import { EmptyState } from '@/components/ui/EmptyState'
import { SkeletonLoader } from '@/components/ui/SkeletonLoader'
import { FieldInput } from './FieldInput'

export function CrudPage({ config }: { config: EntityConfig }) {
  const { t } = useTranslation()
  const { user } = useAuth()
  const { success, error: showError } = useToast()
  const { confirm } = useConfirmDialog()
  const crud = React.useMemo(() => createCrud<Record<string, unknown>, unknown, unknown, string | number>(config.resource), [config.resource])

  const [page, setPage] = React.useState(1)
  const [search, setSearch] = React.useState('')
  const [includeInactive, setIncludeInactive] = React.useState(false)
  const [dialogOpen, setDialogOpen] = React.useState(false)
  const [editingId, setEditingId] = React.useState<string | number | null>(null)
  const [form, setForm] = React.useState<Record<string, unknown>>(config.defaults)
  const [error, setError] = React.useState<string | null>(null)
  const [initialForm, setInitialForm] = React.useState<Record<string, unknown>>(config.defaults)

  const list = crud.useList({ page, pageSize: 20, search: search || undefined, includeInactive })
  const createM = crud.useCreate()
  const updateM = crud.useUpdate()
  const deleteM = crud.useDelete()

  const isAdmin = user?.role === 'Admin'
  const canWrite = !config.readOnly && (!config.adminOnly || isAdmin)

  // Check if form has unsaved changes
  const isDirty = JSON.stringify(form) !== JSON.stringify(initialForm)

  const openCreate = () => {
    setEditingId(null); setForm(config.defaults); setInitialForm(config.defaults); setError(null); setDialogOpen(true)
  }
  const openEdit = (row: Record<string, unknown>) => {
    const values: Record<string, unknown> = { ...config.defaults, ...row }
    for (const f of config.fields) {
      if (f.kind === 'datetime' && typeof values[f.name] === 'string') {
        values[f.name] = (values[f.name] as string).slice(0, 16)
      }
    }
    setEditingId(row.id as string | number); setForm(values); setInitialForm(values); setError(null); setDialogOpen(true)
  }

  const closeDialog = async () => {
    if (isDirty) {
      const confirmed = await confirm({
        title: t('crud.discardTitle'),
        message: t('crud.discardMessage'),
        confirmText: t('crud.discard'),
        cancelText: t('crud.keepEditing'),
        isDangerous: true,
      })
      if (!confirmed) return
    }
    setDialogOpen(false)
  }

  const visibleFields = config.fields.filter((f) => (editingId == null ? f.only !== 'edit' : f.only !== 'create'))
  // Entities whose only fields are create-only (e.g. odontograms: just a patient link) have nothing
  // to edit — an edit dialog would render empty, so don't offer the edit action for them.
  const hasEditFields = config.fields.some((f) => f.only !== 'create')

  // Build the payload for API request with proper type conversions
  const buildPayload = () => {
    const payload: Record<string, unknown> = {}
    for (const f of visibleFields) {
      let v = form[f.name]
      // Convert empty references to null (nullable FK)
      if (f.kind === 'reference' && v === '') v = null
      // Convert datetime string to ISO format
      if (f.kind === 'datetime' && typeof v === 'string' && v) v = new Date(v).toISOString()
      payload[f.name] = v
    }
    console.log('buildPayload:', { visibleFields: visibleFields.map(f => f.name), form, payload })
    return payload
  }

  const submit = async () => {
    setError(null)
    const missing = visibleFields.find((f) => f.required && (form[f.name] === '' || form[f.name] == null))
    if (missing) { setError(`${t(`fields.${missing.name}`, { defaultValue: missing.name })}: ${t('common.required')}`); return }
    try {
      const isCreate = editingId == null
      if (isCreate) await createM.mutateAsync(buildPayload())
      else await updateM.mutateAsync({ id: editingId, dto: buildPayload() })

      // Show success message
      success(isCreate ? t('crud.createSuccess') : t('crud.updateSuccess'))
      setDialogOpen(false)
    } catch (e) {
      const errorMsg = apiErrorMessage(e)
      setError(errorMsg)
      showError(errorMsg)
    }
  }

  const onDelete = async (id: string | number, recordName?: string) => {
    const confirmed = await confirm({
      title: t('crud.deleteTitle'),
      message: recordName
        ? t('crud.deleteConfirmNamed', { name: recordName })
        : t('crud.deleteConfirm'),
      confirmText: t('common.delete'),
      cancelText: t('common.cancel'),
      isDangerous: true,
    })

    if (!confirmed) return

    try {
      await deleteM.mutateAsync(id)
      success(t('crud.deleteSuccess'))
    } catch (e) {
      showError(apiErrorMessage(e))
    }
  }

  // Keyboard shortcuts: Ctrl+S to save
  useKeyboardShortcuts({
    onSave: dialogOpen ? submit : undefined,
  })

  const data = list.data
  const totalPages = data?.totalPages ?? 1

  return (
    <div className="space-y-4">
      <div className="flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
        <h1 className="text-2xl font-semibold">{t(config.navKey)}</h1>
        {canWrite && <Button onClick={openCreate} size="sm" className="w-full sm:w-auto"><Plus size={16} /> {t('common.create')}</Button>}
      </div>

      <div className="flex flex-col gap-3 lg:flex-row lg:items-center">
        <Input placeholder={t('common.search')} value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1) }} className="w-full lg:max-w-xs" />
        <label className="flex items-center gap-2 text-sm text-muted-foreground">
          <input type="checkbox" checked={includeInactive} onChange={(e) => setIncludeInactive(e.target.checked)} />
          <span className="whitespace-nowrap">{t('common.includeInactive')}</span>
        </label>
      </div>

      {/* Desktop view: Traditional data table (hidden on mobile) */}
      <Card className="hidden overflow-hidden md:block">
        <div className="overflow-x-auto">
          {list.isLoading ? (
            <SkeletonLoader count={5} variant="table" />
          ) : data?.items.length === 0 ? (
            <div className="px-4 py-12">
              <EmptyState
                icon={Inbox}
                title={t('common.noData')}
                message={t('crud.emptyMessage')}
                action={canWrite ? { label: t('crud.createNew'), onClick: openCreate } : undefined}
              />
            </div>
          ) : (
            <table className="w-full text-sm">
              <thead className="border-b bg-muted/50">
                <tr>
                  {config.columns.map((c) => (
                    <th key={c.key} className="px-4 py-2 text-left font-medium">{t(`fields.${c.key}`, { defaultValue: c.key })}</th>
                  ))}
                  {canWrite && <th className="px-4 py-2 text-right">{t('common.actions')}</th>}
                </tr>
              </thead>
              <tbody>
                {data?.items.map((row) => (
                  <tr key={String(row.id)} className="border-b last:border-0 hover:bg-muted/30">
                    {config.columns.map((c) => (
                      <td key={c.key} className="px-4 py-2 truncate">{formatCell(row[c.key], c, t)}</td>
                    ))}
                    {canWrite && (
                      <td className="px-4 py-2 text-right">
                        <div className="flex justify-end gap-1">
                          {hasEditFields && <Button variant="ghost" size="icon" onClick={() => openEdit(row)}><Pencil size={15} /></Button>}
                          <Button variant="ghost" size="icon" onClick={() => onDelete(row.id as string | number, String(row.name || row.firstName || row.title || row.id))}><Trash2 size={15} /></Button>
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </Card>

      {/* Mobile/Tablet view: Card-based layout (hidden on desktop) */}
      <div className="md:hidden">
        {list.isLoading ? (
          <SkeletonLoader count={5} variant="card" />
        ) : data?.items.length === 0 ? (
          <EmptyState
            icon={Inbox}
            title={t('common.noData')}
            message={t('crud.emptyMessage')}
            action={canWrite ? { label: t('crud.createNew'), onClick: openCreate } : undefined}
          />
        ) : (
          <div className="grid gap-3">
            {data?.items.map((row) => (
              <Card key={String(row.id)} className="p-4">
                <div className="space-y-3">
                  {/* Display fields as key-value pairs */}
                  {config.columns.map((c) => (
                    <div key={c.key} className="flex justify-between gap-2">
                      <span className="font-medium text-muted-foreground">{t(`fields.${c.key}`, { defaultValue: c.key })}</span>
                      <span className="text-right text-sm">{formatCell(row[c.key], c, t)}</span>
                    </div>
                  ))}
                  {canWrite && (
                    <div className="flex gap-2 border-t pt-3">
                      {hasEditFields && <Button variant="ghost" size="sm" className="flex-1" onClick={() => openEdit(row)}><Pencil size={15} /> {t('common.edit')}</Button>}
                      <Button variant="ghost" size="sm" className="flex-1" onClick={() => onDelete(row.id as string | number, String(row.name || row.firstName || row.title || row.id))}><Trash2 size={15} /> {t('common.delete')}</Button>
                    </div>
                  )}
                </div>
              </Card>
            ))}
          </div>
        )}
      </div>

      {/* Pagination */}
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <span className="text-sm text-muted-foreground">{t('common.page')} {page} {t('common.of')} {totalPages}</span>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>{t('common.previous')}</Button>
          <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>{t('common.next')}</Button>
        </div>
      </div>

      {/* Dialog */}
      <Dialog open={dialogOpen} onClose={closeDialog}
        title={editingId == null ? t('common.create') : t('common.edit')}>
        <div className="max-h-[60vh] space-y-3 overflow-y-auto pr-1">
          {visibleFields.map((f) => (
            <FieldInput key={f.name} field={f} value={form[f.name]}
              onChange={(v) => {
                console.log('CrudPage onChange:', { field: f.name, oldValue: form[f.name], newValue: v })
                setForm((prev) => ({ ...prev, [f.name]: v }))
              }} />
          ))}
          {error && <p className="text-sm text-destructive">{error}</p>}
        </div>
        <div className="mt-4 space-y-3">
          {isDirty && <p className="text-xs text-amber-600 font-medium">⚠ {t('crud.unsavedChanges')}</p>}
          <div className="flex flex-col gap-2 sm:flex-row sm:justify-end">
            <Button variant="outline" onClick={closeDialog} className="w-full sm:w-auto" disabled={createM.isPending || updateM.isPending}>{t('common.cancel')}</Button>
            <Button onClick={submit} isLoading={createM.isPending || updateM.isPending} className="w-full sm:w-auto">{t('common.save')} (Ctrl+S)</Button>
          </div>
        </div>
      </Dialog>
    </div>
  )
}

function formatCell(value: unknown, col: ColumnConfig, t: TFunction) {
  if (value == null || value === '') return '—'
  switch (col.kind) {
    case 'bool': return value ? t('common.active') : t('common.inactive')
    case 'enum': return col.enumName ? t(`${col.enumName}.${value}`, { defaultValue: String(value) }) : String(value)
    case 'money': return Number(value).toLocaleString(undefined, { style: 'currency', currency: 'USD' })
    case 'date': return String(value)
    case 'datetime': return new Date(String(value)).toLocaleString()
    default: return String(value)
  }
}
