import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'
import es from './locales/es.json'
import en from './locales/en.json'
import { getSessionUser } from '@/lib/auth/session'

const stored = getSessionUser()?.preferredLanguage
const initialLang = stored || localStorage.getItem('aespejo.lang') || 'es'

i18n.use(initReactI18next).init({
  resources: {
    es: { translation: es },
    en: { translation: en },
  },
  lng: initialLang,
  fallbackLng: 'es',
  interpolation: { escapeValue: false },
})

i18n.on('languageChanged', (lng) => localStorage.setItem('aespejo.lang', lng))

export default i18n
