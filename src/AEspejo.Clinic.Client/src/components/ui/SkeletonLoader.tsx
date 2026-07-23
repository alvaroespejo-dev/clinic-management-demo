import { Card } from './primitives'
import { cn } from '@/lib/utils'

interface SkeletonLoaderProps {
  count?: number
  variant?: 'table' | 'card'
}

// Animated skeleton placeholder
function SkeletonPulse() {
  return (
    <div className="h-4 w-3/4 rounded-md bg-muted animate-pulse" />
  )
}

export function SkeletonLoader({ count = 5, variant = 'table' }: SkeletonLoaderProps) {
  if (variant === 'card') {
    return (
      <div className="grid gap-3">
        {Array.from({ length: count }).map((_, i) => (
          <Card key={i} className="p-4 space-y-3">
            <SkeletonPulse />
            <SkeletonPulse />
            <SkeletonPulse />
          </Card>
        ))}
      </div>
    )
  }

  return (
    <div className="space-y-2">
      {Array.from({ length: count }).map((_, i) => (
        <div key={i} className="flex gap-4 p-4 border-b">
          <SkeletonPulse />
          <SkeletonPulse />
          <SkeletonPulse />
        </div>
      ))}
    </div>
  )
}
