import { useEffect } from 'react'

interface KeyboardShortcutsConfig {
  onSave?: () => void
  onEscape?: () => void
}

export function useKeyboardShortcuts({ onSave, onEscape }: KeyboardShortcutsConfig) {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Ctrl+S or Cmd+S to save
      if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault()
        onSave?.()
      }

      // Escape to cancel
      if (e.key === 'Escape') {
        onEscape?.()
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [onSave, onEscape])
}
