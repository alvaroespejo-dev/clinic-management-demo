import * as React from 'react'

export interface ConfirmOptions {
  title: string
  message: string
  confirmText?: string
  cancelText?: string
  isDangerous?: boolean
}

interface ConfirmContextType {
  confirm: (options: ConfirmOptions) => Promise<boolean>
  isOpen: boolean
  options: ConfirmOptions | null
  closeConfirm: (confirmed: boolean) => void
}

const ConfirmContext = React.createContext<ConfirmContextType | undefined>(undefined)

export function ConfirmProvider({ children }: { children: React.ReactNode }) {
  const [isOpen, setIsOpen] = React.useState(false)
  const [options, setOptions] = React.useState<ConfirmOptions | null>(null)
  const resolveRef = React.useRef<((value: boolean) => void) | null>(null)

  const confirm = React.useCallback((confirmOptions: ConfirmOptions): Promise<boolean> => {
    return new Promise((resolve) => {
      setOptions(confirmOptions)
      setIsOpen(true)
      resolveRef.current = resolve
    })
  }, [])

  const closeConfirm = React.useCallback((confirmed: boolean) => {
    setIsOpen(false)
    setOptions(null)
    if (resolveRef.current) {
      resolveRef.current(confirmed)
      resolveRef.current = null
    }
  }, [])

  const value: ConfirmContextType = { confirm, isOpen, options, closeConfirm }

  return (
    <ConfirmContext.Provider value={value}>
      {children}
    </ConfirmContext.Provider>
  )
}

export function useConfirmDialog() {
  const context = React.useContext(ConfirmContext)
  if (context === undefined) {
    throw new Error('useConfirmDialog must be used within ConfirmProvider')
  }
  return context
}
