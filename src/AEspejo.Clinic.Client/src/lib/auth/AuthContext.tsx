import * as React from 'react'
import i18n from '@/lib/i18n'
import { api } from '@/lib/api/client'
import type { LoginRequest, LoginResponse } from '@/lib/api/types'
import { clearSession, getSessionUser, saveSession, type SessionUser } from './session'

interface AuthContextValue {
  user: SessionUser | null
  login: (req: LoginRequest) => Promise<void>
  logout: () => void
}

const AuthContext = React.createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = React.useState<SessionUser | null>(() => getSessionUser())

  const login = React.useCallback(async (req: LoginRequest) => {
    const { data } = await api.post<LoginResponse>('/api/auth/login', req)
    saveSession(data)
    setUser(getSessionUser())
    if (data.preferredLanguage) i18n.changeLanguage(data.preferredLanguage)
  }, [])

  const logout = React.useCallback(() => {
    clearSession()
    setUser(null)
    window.location.href = '/login'
  }, [])

  return <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = React.useContext(AuthContext)
  if (!ctx) throw new Error('useAuth debe usarse dentro de AuthProvider')
  return ctx
}
