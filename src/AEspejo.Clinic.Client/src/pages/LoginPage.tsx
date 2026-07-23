import * as React from 'react'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useAuth } from '@/lib/auth/AuthContext'
import { apiErrorMessage } from '@/lib/api/client'
import { Button, Input, Label, Card } from '@/components/ui/primitives'
import { LanguageSwitcher } from '@/components/shared/LanguageSwitcher'

export function LoginPage() {
  const { t } = useTranslation()
  const { login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = React.useState('admin@demo.local')
  const [password, setPassword] = React.useState('Admin12345')
  const [error, setError] = React.useState<string | null>(null)
  const [loading, setLoading] = React.useState(false)

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true); setError(null)
    try {
      await login({ email, password })
      navigate('/')
    } catch (err) {
      setError(apiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/30 p-4">
      <Card className="w-full max-w-sm p-6">
        <div className="mb-4 flex items-center justify-between">
          <h1 className="text-xl font-semibold">{t('common.appName')}</h1>
          <LanguageSwitcher />
        </div>
        <p className="mb-4 text-sm text-muted-foreground">{t('login.subtitle')}</p>
        <form onSubmit={submit} className="space-y-3">
          <div className="space-y-1">
            <Label htmlFor="email">{t('login.email')}</Label>
            <Input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div className="space-y-1">
            <Label htmlFor="password">{t('login.password')}</Label>
            <Input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
          </div>
          {error && <p className="text-sm text-destructive">{error}</p>}
          <Button type="submit" className="w-full" disabled={loading}>{t('login.submit')}</Button>
        </form>
      </Card>
    </div>
  )
}
