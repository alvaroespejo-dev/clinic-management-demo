import * as React from 'react'
import { useTranslation } from 'react-i18next'
import { Upload } from 'lucide-react'
import { useConfig } from '@/lib/config/ConfigContext'
import { useToast } from '@/lib/toast/ToastContext'
import { Button, Input, Card } from '@/components/ui/primitives'

export function ConfigPage() {
  const { t } = useTranslation()
  const { config, updateConfig, loading } = useConfig()
  const { success, error: showError } = useToast()
  const [name, setName] = React.useState('')
  const [logoUrl, setLogoUrl] = React.useState('')
  const [saving, setSaving] = React.useState(false)

  // Initialize form with loaded config
  React.useEffect(() => {
    if (config) {
      setName(config.name || '')
      setLogoUrl(config.logoUrl || '')
    }
  }, [config])

  // Handle logo file upload and convert to base64
  const handleLogoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      const reader = new FileReader()
      reader.onload = (evt) => {
        setLogoUrl(evt.target?.result as string)
      }
      reader.readAsDataURL(file)
    }
  }

  // Submit form and update clinic configuration
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!name.trim()) {
      showError(t('settings.nameRequired'))
      return
    }

    try {
      setSaving(true)
      await updateConfig({
        id: config?.id || '',
        name,
        logoUrl,
      })
      success(t('settings.saveSuccess'))
    } catch (err) {
      showError(t('settings.saveError'))
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return <div className="text-center text-muted-foreground">{t('common.loading')}</div>
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">{t('settings.title')}</h1>
        <p className="mt-1 text-sm text-muted-foreground">{t('settings.subtitle')}</p>
      </div>

      <Card className="p-6">
        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Logo upload section */}
          <div>
            <label className="block text-sm font-medium">{t('settings.logo')}</label>
            <div className="mt-3 flex flex-col gap-4 sm:flex-row">
              {logoUrl && (
                <div className="flex h-24 w-24 items-center justify-center rounded-lg border bg-muted">
                  <img src={logoUrl} alt="logo" className="h-20 w-20 object-contain" />
                </div>
              )}
              <div className="flex-1">
                <label className="inline-flex cursor-pointer items-center gap-2 rounded-md border border-dashed px-4 py-6 text-sm hover:bg-accent">
                  <Upload size={16} />
                  <span>{t('settings.uploadLogo')}</span>
                  <input
                    type="file"
                    accept="image/*"
                    onChange={handleLogoUpload}
                    className="hidden"
                  />
                </label>
              </div>
            </div>
          </div>

          {/* Clinic name input section */}
          <div>
            <label className="block text-sm font-medium mb-2">
              {t('settings.clinicName')}
            </label>
            <Input
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder={t('settings.clinicNamePlaceholder')}
              className="w-full"
              required
            />
          </div>

          {/* Submit button */}
          <div className="flex gap-3 pt-4">
            <Button type="submit" isLoading={saving} disabled={saving}>
              {saving ? t('settings.saving') : t('settings.saveChanges')}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  )
}
