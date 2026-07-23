import * as React from 'react'
import { useTranslation } from 'react-i18next'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Pencil, Trash2, Plus } from 'lucide-react'
import { api, apiErrorMessage } from '@/lib/api/client'
import { useAuth } from '@/lib/auth/AuthContext'
import type { PagedResult, ServiceCatalog, ServiceTranslation } from '@/lib/api/types'
import { Button, Input, Card, Label } from '@/components/ui/primitives'
import { Dialog } from '@/components/ui/dialog'

const EMPTY_TRANSLATIONS: ServiceTranslation[] = [
  { languageCode: 'es', name: '', description: '', category: '' },
  { languageCode: 'en', name: '', description: '', category: '' },
]

export function ServicesPage() {
  const { t, i18n } = useTranslation()
  const { user } = useAuth()
  const qc = useQueryClient()
  const isAdmin = user?.role === 'Admin'

  const [search, setSearch] = React.useState('')
  const [dialogOpen, setDialogOpen] = React.useState(false)
  const [editingId, setEditingId] = React.useState<string | null>(null)
  const [code, setCode] = React.useState('')
  const [price, setPrice] = React.useState(0)
  const [isActive, setIsActive] = React.useState(true)
  const [translations, setTranslations] = React.useState<ServiceTranslation[]>(EMPTY_TRANSLATIONS)
  const [error, setError] = React.useState<string | null>(null)

  const list = useQuery({
    queryKey: ['services', 'list', search, i18n.language],
    queryFn: async () => {
      const { data } = await api.get<PagedResult<ServiceCatalog>>('/api/services', {
        params: { pageSize: 100, search: search || undefined, lang: i18n.language },
      })
      return data
    },
  })

  const save = useMutation({
    mutationFn: async () => {
      const body = { code, defaultPrice: price, isActive, translations }
      if (editingId) await api.put(`/api/services/${editingId}`, body)
      else await api.post('/api/services', body)
    },
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['services'] }); setDialogOpen(false) },
    onError: (e) => setError(apiErrorMessage(e)),
  })

  const remove = useMutation({
    mutationFn: async (id: string) => { await api.delete(`/api/services/${id}`) },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['services'] }),
  })

  const openCreate = () => {
    setEditingId(null); setCode(''); setPrice(0); setIsActive(true)
    setTranslations(EMPTY_TRANSLATIONS.map((x) => ({ ...x }))); setError(null); setDialogOpen(true)
  }
  const openEdit = (s: ServiceCatalog) => {
    setEditingId(s.id); setCode(s.code); setPrice(s.defaultPrice); setIsActive(s.isActive)
    const base = EMPTY_TRANSLATIONS.map((e) => s.translations.find((x) => x.languageCode === e.languageCode) ?? { ...e })
    setTranslations(base); setError(null); setDialogOpen(true)
  }

  const setTr = (i: number, key: keyof ServiceTranslation, value: string) =>
    setTranslations((prev) => prev.map((x, idx) => (idx === i ? { ...x, [key]: value } : x)))

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">{t('nav.services')}</h1>
        {isAdmin && <Button onClick={openCreate}><Plus size={16} /> {t('common.create')}</Button>}
      </div>

      <Input placeholder={t('common.search')} value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-xs" />

      <Card className="overflow-hidden">
        <table className="w-full text-sm">
          <thead className="border-b bg-muted/50">
            <tr>
              <th className="px-4 py-2 text-left font-medium">{t('fields.code')}</th>
              <th className="px-4 py-2 text-left font-medium">{t('fields.name')}</th>
              <th className="px-4 py-2 text-left font-medium">{t('fields.category')}</th>
              <th className="px-4 py-2 text-left font-medium">{t('fields.defaultPrice')}</th>
              <th className="px-4 py-2 text-left font-medium">{t('fields.isActive')}</th>
              {isAdmin && <th className="px-4 py-2 text-right">{t('common.actions')}</th>}
            </tr>
          </thead>
          <tbody>
            {list.data?.items.map((s) => (
              <tr key={s.id} className="border-b last:border-0 hover:bg-muted/30">
                <td className="px-4 py-2">{s.code}</td>
                <td className="px-4 py-2">{s.name}</td>
                <td className="px-4 py-2">{s.category}</td>
                <td className="px-4 py-2">{Number(s.defaultPrice).toLocaleString(undefined, { style: 'currency', currency: 'USD' })}</td>
                <td className="px-4 py-2">{s.isActive ? t('common.active') : t('common.inactive')}</td>
                {isAdmin && (
                  <td className="px-4 py-2 text-right">
                    <div className="flex justify-end gap-1">
                      <Button variant="ghost" size="icon" onClick={() => openEdit(s)}><Pencil size={15} /></Button>
                      <Button variant="ghost" size="icon" onClick={() => remove.mutate(s.id)}><Trash2 size={15} /></Button>
                    </div>
                  </td>
                )}
              </tr>
            ))}
            {list.data?.items.length === 0 && (
              <tr><td className="px-4 py-6 text-muted-foreground" colSpan={6}>{t('common.noData')}</td></tr>
            )}
          </tbody>
        </table>
      </Card>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}
        title={editingId ? t('common.edit') : t('common.create')} className="max-w-2xl">
        <div className="max-h-[65vh] space-y-3 overflow-y-auto pr-1">
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1">
              <Label>{t('fields.code')} *</Label>
              <Input value={code} onChange={(e) => setCode(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label>{t('fields.defaultPrice')} *</Label>
              <Input type="number" value={price} onChange={(e) => setPrice(Number(e.target.value))} />
            </div>
          </div>
          {editingId && (
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} /> {t('fields.isActive')}
            </label>
          )}

          <div className="space-y-2">
            <Label>{t('fields.languageCode')} — {t('nav.services')}</Label>
            {translations.map((tr, i) => (
              <div key={tr.languageCode} className="rounded-md border p-3">
                <p className="mb-2 text-xs font-semibold uppercase text-muted-foreground">{tr.languageCode}</p>
                <div className="grid grid-cols-2 gap-2">
                  <Input placeholder={t('fields.name')} value={tr.name} onChange={(e) => setTr(i, 'name', e.target.value)} />
                  <Input placeholder={t('fields.category')} value={tr.category} onChange={(e) => setTr(i, 'category', e.target.value)} />
                  <Input className="col-span-2" placeholder={t('fields.description')} value={tr.description ?? ''} onChange={(e) => setTr(i, 'description', e.target.value)} />
                </div>
              </div>
            ))}
          </div>
          {error && <p className="text-sm text-destructive">{error}</p>}
        </div>
        <div className="mt-4 flex justify-end gap-2">
          <Button variant="outline" onClick={() => setDialogOpen(false)}>{t('common.cancel')}</Button>
          <Button onClick={() => save.mutate()} disabled={save.isPending}>{t('common.save')}</Button>
        </div>
      </Dialog>
    </div>
  )
}
