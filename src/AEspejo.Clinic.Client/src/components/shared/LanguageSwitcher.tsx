import { useTranslation } from 'react-i18next'
import { Select } from '@/components/ui/primitives'

export function LanguageSwitcher() {
  const { i18n } = useTranslation()
  return (
    <Select value={i18n.language} onChange={(e) => i18n.changeLanguage(e.target.value)} className="w-28">
      <option value="es">Español</option>
      <option value="en">English</option>
    </Select>
  )
}
