import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { AlertCircle } from 'lucide-react'
import { Button, Card } from '@/components/ui/primitives'

export function NotFoundPage() {
  const navigate = useNavigate()
  const { t } = useTranslation()

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="max-w-md p-6 text-center">
        <AlertCircle size={48} className="mx-auto mb-4 text-muted-foreground" />
        <h1 className="mb-2 text-3xl font-semibold">404</h1>
        <p className="mb-2 text-lg font-medium">{t('notFound.title')}</p>
        <p className="mb-6 text-sm text-muted-foreground">
          {t('notFound.message')}
        </p>
        <div className="flex flex-col gap-2">
          <Button onClick={() => navigate('/')}>{t('notFound.goHome')}</Button>
          <Button variant="outline" onClick={() => navigate(-1)}>{t('notFound.goBack')}</Button>
        </div>
      </Card>
    </div>
  )
}
