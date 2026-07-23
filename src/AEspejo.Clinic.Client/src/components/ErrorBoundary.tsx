import * as React from 'react'
import { useTranslation } from 'react-i18next'
import { AlertTriangle } from 'lucide-react'
import { Button, Card } from '@/components/ui/primitives'

interface ErrorBoundaryProps {
  children: React.ReactNode
}

// Functional fallback so we can use the i18n hook (class components can't).
function ErrorFallback({ error, onReset }: { error: Error | null; onReset: () => void }) {
  const { t } = useTranslation()
  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="max-w-md p-6 text-center">
        <AlertTriangle size={48} className="mx-auto mb-4 text-destructive" />
        <h1 className="mb-2 text-2xl font-semibold">{t('error.title')}</h1>
        <p className="mb-4 text-sm text-muted-foreground">
          {t('error.message')}
        </p>
        {import.meta.env.DEV && error && (
          <pre className="mb-4 rounded-md bg-muted p-2 text-left text-xs overflow-auto">
            {error.message}
          </pre>
        )}
        <Button onClick={onReset}>{t('error.retry')}</Button>
      </Card>
    </div>
  )
}

interface ErrorBoundaryState {
  hasError: boolean
  error: Error | null
}

export class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props)
    this.state = { hasError: false, error: null }
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { hasError: true, error }
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error('Error caught by boundary:', error, errorInfo)
  }

  resetError = () => {
    this.setState({ hasError: false, error: null })
  }

  render() {
    if (this.state.hasError) {
      return <ErrorFallback error={this.state.error} onReset={this.resetError} />
    }

    return this.props.children
  }
}
