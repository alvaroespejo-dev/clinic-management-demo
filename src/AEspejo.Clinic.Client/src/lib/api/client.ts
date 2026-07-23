import axios from 'axios'
import { getToken, getTenant, clearSession } from '@/lib/auth/session'

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL as string,
})

// Injects Bearer + X-Tenant into every request.
api.interceptors.request.use((config) => {
  const token = getToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  const tenant = getTenant()
  if (tenant) config.headers['X-Tenant'] = tenant
  return config
})

// On 401: clears the session and redirects to login (unless already on login).
api.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401 && !window.location.pathname.startsWith('/login')) {
      clearSession()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

/** Extracts a readable error message from an API response. */
export function apiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (error.code === 'ERR_NETWORK') return 'No se pudo contactar el servidor. Verifica que la API esté corriendo.'
    const data = error.response?.data as
      | { errors?: string[] | Record<string, string[]>; detail?: string; title?: string }
      | undefined
    if (data?.errors) {
      // Custom format (string[]) or ASP.NET validation (field → messages dictionary).
      const list = Array.isArray(data.errors) ? data.errors : Object.values(data.errors).flat()
      if (list.length) return list.join(' ')
    }
    if (data?.detail) return data.detail
    if (data?.title) return data.title
    return error.message
  }
  return 'Error inesperado.'
}
