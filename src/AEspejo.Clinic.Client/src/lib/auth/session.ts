import type { LoginResponse } from '@/lib/api/types'

const TOKEN_KEY = 'aespejo.token'
const USER_KEY = 'aespejo.user'

export interface SessionUser {
  userId: string
  fullName: string
  role: LoginResponse['role']
  preferredLanguage: string
}

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function getSessionUser(): SessionUser | null {
  const raw = localStorage.getItem(USER_KEY)
  return raw ? (JSON.parse(raw) as SessionUser) : null
}

export function saveSession(login: LoginResponse) {
  localStorage.setItem(TOKEN_KEY, login.token)
  const user: SessionUser = {
    userId: login.userId,
    fullName: login.fullName,
    role: login.role,
    preferredLanguage: login.preferredLanguage,
  }
  localStorage.setItem(USER_KEY, JSON.stringify(user))
}

export function clearSession() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(USER_KEY)
}

/** Active tenant: comes from VITE_DEV_TENANT in dev; derived from the subdomain in prod. */
export function getTenant(): string {
  const devTenant = import.meta.env.VITE_DEV_TENANT as string | undefined
  if (devTenant) return devTenant
  const host = window.location.hostname
  const parts = host.split('.')
  return parts.length >= 2 ? parts[0] : ''
}
