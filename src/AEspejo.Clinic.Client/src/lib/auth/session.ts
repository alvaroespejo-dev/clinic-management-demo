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

/**
 * Active tenant: an explicit build-time override (VITE_TENANT for any env, or VITE_DEV_TENANT for dev)
 * wins; otherwise it's derived from the subdomain (e.g. demo.example.com -> "demo").
 * The override is needed for single-tenant hosts whose subdomain isn't a tenant (e.g. an Azure
 * Static Web App at blue-forest-xxxx.azurestaticapps.net).
 */
export function getTenant(): string {
  const configured = (import.meta.env.VITE_TENANT ?? import.meta.env.VITE_DEV_TENANT) as string | undefined
  if (configured) return configured
  const host = window.location.hostname
  const parts = host.split('.')
  return parts.length >= 2 ? parts[0] : ''
}
