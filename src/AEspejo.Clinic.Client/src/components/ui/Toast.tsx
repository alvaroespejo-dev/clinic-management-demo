import * as React from 'react'
import { X, CheckCircle2, AlertCircle, AlertTriangle, Info } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { useToast, type Toast as ToastType } from '@/lib/toast/ToastContext'
import { cn } from '@/lib/utils'

function ToastItem({ toast }: { toast: ToastType }) {
  const { t } = useTranslation()
  const { dismiss } = useToast()

  const styles = {
    success: {
      bg: 'bg-green-50 border-green-200',
      text: 'text-green-800',
      icon: CheckCircle2,
      color: 'text-green-600',
    },
    error: {
      bg: 'bg-red-50 border-red-200',
      text: 'text-red-800',
      icon: AlertCircle,
      color: 'text-red-600',
    },
    warning: {
      bg: 'bg-yellow-50 border-yellow-200',
      text: 'text-yellow-800',
      icon: AlertTriangle,
      color: 'text-yellow-600',
    },
    info: {
      bg: 'bg-blue-50 border-blue-200',
      text: 'text-blue-800',
      icon: Info,
      color: 'text-blue-600',
    },
  }

  const style = styles[toast.type]
  const Icon = style.icon

  return (
    <div
      className={cn(
        'flex items-start gap-3 rounded-lg border px-4 py-3 shadow-lg animate-in fade-in slide-in-from-right-4',
        style.bg,
        style.text
      )}
      role="status"
      aria-live="polite"
    >
      <Icon size={20} className={cn('mt-0.5 shrink-0', style.color)} />
      <p className="flex-1 text-sm font-medium">{toast.message}</p>
      {toast.duration === 0 && (
        <button
          onClick={() => dismiss(toast.id)}
          className="shrink-0 hover:opacity-70"
          aria-label={t('common.dismissNotification')}
        >
          <X size={18} />
        </button>
      )}
    </div>
  )
}

export function ToastContainer() {
  const { toasts } = useToast()

  return (
    <div className="fixed bottom-0 right-0 z-50 flex flex-col gap-2 p-4 pointer-events-none">
      <div className="space-y-2 pointer-events-auto">
        {toasts.map((toast) => (
          <ToastItem key={toast.id} toast={toast} />
        ))}
      </div>
    </div>
  )
}
