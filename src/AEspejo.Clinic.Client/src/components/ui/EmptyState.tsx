import { LucideIcon } from 'lucide-react'
import { Button } from './primitives'

interface EmptyStateProps {
  icon?: LucideIcon
  title: string
  message?: string
  action?: {
    label: string
    onClick: () => void
  }
}

export function EmptyState({ icon: Icon, title, message, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center py-12 px-4">
      {Icon && <Icon size={48} className="text-muted-foreground mb-4 opacity-40" />}
      <h3 className="text-lg font-semibold mb-2">{title}</h3>
      {message && <p className="text-sm text-muted-foreground mb-4 text-center max-w-sm">{message}</p>}
      {action && (
        <Button onClick={action.onClick} size="sm">
          {action.label}
        </Button>
      )}
    </div>
  )
}
