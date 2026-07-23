import { useTranslation } from 'react-i18next'
import type { FieldConfig } from '@/features/config'
import { ENUM_VALUES } from '@/lib/api/types'
import { Input, Textarea, Select, Label } from '@/components/ui/primitives'
import { ReferenceSelect } from './ReferenceSelect'

interface Props {
  field: FieldConfig
  value: unknown
  onChange: (value: unknown) => void
}

export function FieldInput({ field, value, onChange }: Props) {
  const { t } = useTranslation()
  const label = t(`fields.${field.name}`, { defaultValue: field.name })
  const id = `f_${field.name}`

  const control = () => {
    switch (field.kind) {
      case 'textarea':
        return <Textarea id={id} value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} />
      case 'number':
        return (
          <Input id={id} type="number" value={value === null || value === undefined ? '' : String(value)}
            onChange={(e) => onChange(e.target.value === '' ? null : Number(e.target.value))} />
        )
      case 'checkbox':
        return (
          <input id={id} type="checkbox" className="h-4 w-4" checked={Boolean(value)}
            onChange={(e) => onChange(e.target.checked)} />
        )
      case 'date':
        return <Input id={id} type="date" value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} />
      case 'datetime':
        return (
          <Input id={id} type="datetime-local" value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} />
        )
      case 'password':
        return <Input id={id} type="password" value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} />
      case 'enum': {
        const options = field.enumName ? ENUM_VALUES[field.enumName] : []
        return (
          <Select id={id} value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)}>
            {options.map((opt) => (
              <option key={opt} value={opt}>{t(`${field.enumName}.${opt}`, { defaultValue: opt })}</option>
            ))}
          </Select>
        )
      }
      case 'reference':
        return (
          <ReferenceSelect id={id} resource={field.reference!.resource} label={field.reference!.label}
            allowEmpty={!field.required} value={(value as string) ?? ''} onChange={onChange} />
        )
      default:
        return <Input id={id} value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} />
    }
  }

  if (field.kind === 'checkbox') {
    return (
      <div className="flex items-center gap-2">
        {control()}
        <Label htmlFor={id}>{label}</Label>
      </div>
    )
  }

  return (
    <div className="space-y-1">
      <Label htmlFor={id}>{label}{field.required && <span className="text-destructive"> *</span>}</Label>
      {control()}
    </div>
  )
}
