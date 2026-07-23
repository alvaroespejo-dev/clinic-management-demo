import { useTranslation } from 'react-i18next'
import { useAuth } from '@/lib/auth/AuthContext'
import { Card } from '@/components/ui/primitives'

export function DashboardPage() {
  const { t } = useTranslation()
  const { user } = useAuth()
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">{t('nav.dashboard')}</h1>
      <Card className="p-6">
        <p className="text-lg">{user?.fullName}</p>
        <p className="text-sm text-muted-foreground">
          {t(`UserRole.${user?.role}`, { defaultValue: user?.role ?? '' })}
        </p>
      </Card>
    </div>
  )
}
