import * as React from 'react'
import { api } from '@/lib/api/client'

export interface OrgConfig {
  id: string
  name: string
  logoUrl?: string
}

interface ConfigContextType {
  config: OrgConfig | null
  loading: boolean
  updateConfig: (config: OrgConfig) => Promise<void>
  refreshConfig: () => Promise<void>
}

const ConfigContext = React.createContext<ConfigContextType | undefined>(undefined)

export function ConfigProvider({ children }: { children: React.ReactNode }) {
  const [config, setConfig] = React.useState<OrgConfig | null>(null)
  const [loading, setLoading] = React.useState(true)

  // Fetch organization configuration from the API
  const refreshConfig = React.useCallback(async () => {
    try {
      setLoading(true)
      const res = await api.get<OrgConfig>('/api/config')
      setConfig(res.data)
    } catch (e) {
      console.error('Failed to load config:', e)
    } finally {
      setLoading(false)
    }
  }, [])

  // Update organization configuration via API
  const updateConfig = React.useCallback(async (newConfig: OrgConfig) => {
    try {
      await api.put<OrgConfig>('/api/config', newConfig)
      setConfig(newConfig)
    } catch (e) {
      console.error('Failed to update config:', e)
      throw e
    }
  }, [])

  React.useEffect(() => {
    refreshConfig()
  }, [refreshConfig])

  return (
    <ConfigContext.Provider value={{ config, loading, updateConfig, refreshConfig }}>
      {children}
    </ConfigContext.Provider>
  )
}

export function useConfig() {
  const context = React.useContext(ConfigContext)
  if (context === undefined) {
    throw new Error('useConfig must be used within ConfigProvider')
  }
  return context
}
