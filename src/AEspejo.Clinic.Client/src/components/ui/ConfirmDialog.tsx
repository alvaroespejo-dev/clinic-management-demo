import { useConfirmDialog } from '@/lib/dialog/useConfirmDialog'
import { Dialog } from './dialog'
import { Button } from './primitives'

export function ConfirmDialogComponent() {
  const { isOpen, options, closeConfirm } = useConfirmDialog()

  if (!isOpen || !options) return null

  const handleConfirm = () => closeConfirm(true)
  const handleCancel = () => closeConfirm(false)

  return (
    <Dialog
      open={isOpen}
      onClose={handleCancel}
      title={options.title}
    >
      <div className="space-y-4">
        <p className="text-sm text-muted-foreground">{options.message}</p>
        <div className="flex flex-col-reverse gap-2 sm:flex-row sm:justify-end">
          <Button
            variant="outline"
            onClick={handleCancel}
            className="w-full sm:w-auto"
          >
            {options.cancelText || 'Cancel'}
          </Button>
          <Button
            variant={options.isDangerous ? 'destructive' : 'default'}
            onClick={handleConfirm}
            className="w-full sm:w-auto"
          >
            {options.confirmText || 'Confirm'}
          </Button>
        </div>
      </div>
    </Dialog>
  )
}
