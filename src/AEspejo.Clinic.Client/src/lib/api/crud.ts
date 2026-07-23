import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from './client'
import type { PagedQuery, PagedResult } from './types'

/**
 * Generic CRUD factory (mirror of the backend's CrudControllerBase).
 * Returns TanStack Query hooks for an entity, given its REST resource.
 */
export function createCrud<TResponse, TCreate, TUpdate, TId = string>(resource: string) {
  const base = `/api/${resource}`
  const keys = {
    all: [resource] as const,
    list: (q: PagedQuery) => [resource, 'list', q] as const,
    detail: (id: TId) => [resource, 'detail', id] as const,
  }

  function useList(query: PagedQuery = {}) {
    return useQuery({
      queryKey: keys.list(query),
      queryFn: async () => {
        const { data } = await api.get<PagedResult<TResponse>>(base, { params: query })
        return data
      },
    })
  }

  function useGet(id: TId, enabled = true) {
    return useQuery({
      queryKey: keys.detail(id),
      enabled: enabled && id != null,
      queryFn: async () => {
        const { data } = await api.get<TResponse>(`${base}/${id}`)
        return data
      },
    })
  }

  function useCreate() {
    const qc = useQueryClient()
    return useMutation({
      mutationFn: async (dto: TCreate) => {
        const { data } = await api.post<TResponse>(base, dto)
        return data
      },
      onSuccess: () => qc.invalidateQueries({ queryKey: keys.all }),
    })
  }

  function useUpdate() {
    const qc = useQueryClient()
    return useMutation({
      mutationFn: async ({ id, dto }: { id: TId; dto: TUpdate }) => {
        const { data } = await api.put<TResponse>(`${base}/${id}`, dto)
        return data
      },
      onSuccess: () => qc.invalidateQueries({ queryKey: keys.all }),
    })
  }

  function useDelete() {
    const qc = useQueryClient()
    return useMutation({
      mutationFn: async (id: TId) => {
        await api.delete(`${base}/${id}`)
      },
      onSuccess: () => qc.invalidateQueries({ queryKey: keys.all }),
    })
  }

  return { resource, base, keys, useList, useGet, useCreate, useUpdate, useDelete }
}

export type CrudApi<TResponse, TCreate, TUpdate, TId = string> = ReturnType<
  typeof createCrud<TResponse, TCreate, TUpdate, TId>
>
