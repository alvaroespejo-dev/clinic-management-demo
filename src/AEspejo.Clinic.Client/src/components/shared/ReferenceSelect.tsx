import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api/client'
import type { PagedResult } from '@/lib/api/types'
import { Select } from '@/components/ui/primitives'

interface Props {
  resource: string
  label: (row: Record<string, unknown>) => string
  value: string
  onChange: (value: string) => void
  allowEmpty?: boolean
  id?: string
}

/** Dropdown that loads its options from an API endpoint (for FK fields). */
export function ReferenceSelect({ resource, label, value, onChange, allowEmpty, id }: Props) {
  const { data } = useQuery({
    queryKey: [resource, 'options'],
    queryFn: async () => {
      const res = await api.get<PagedResult<Record<string, unknown>>>(`/api/${resource}`, {
        params: { page: 1, pageSize: 200 },
      })
      return res.data.items
    },
  })

  // Check whether the current value is in the option list
  const valueExistsInData = data?.some((row) => String(row.id) === value)

  // Render the empty option when:
  // 1. allowEmpty is true (optional field), OR
  // 2. The value is falsy (empty or null), OR
  // 3. The value is NOT in the available data list (possible inconsistency)
  const showEmpty = allowEmpty || !value || (data && !valueExistsInData)

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    console.log('ReferenceSelect onChange:', { resource, oldValue: value, newValue: e.target.value })
    onChange(e.target.value)
  }

  return (
    <Select id={id} value={value ?? ''} onChange={handleChange}>
      {showEmpty && <option value="">—</option>}
      {(data ?? []).map((row) => (
        <option key={String(row.id)} value={String(row.id)}>
          {label(row)}
        </option>
      ))}
    </Select>
  )
}
