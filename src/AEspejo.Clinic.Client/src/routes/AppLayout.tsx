import * as React from 'react'
import { NavLink, Outlet, Navigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { LayoutDashboard, Menu, X, Settings } from 'lucide-react'
import { useAuth } from '@/lib/auth/AuthContext'
import { useConfig } from '@/lib/config/ConfigContext'
import { ENTITIES } from '@/features/registry'
import { Button } from '@/components/ui/primitives'
import { LanguageSwitcher } from '@/components/shared/LanguageSwitcher'
import { cn } from '@/lib/utils'

const GROUP_ORDER = ['nav.groupOrg', 'nav.groupPatients', 'nav.groupClinical', 'nav.groupBilling', 'nav.groupCatalog', 'nav.groupSystem']

export function AppLayout() {
  const { t } = useTranslation()
  const { user, logout } = useAuth()
  const { config } = useConfig()
  const [sidebarOpen, setSidebarOpen] = React.useState(true)

  if (!user) return <Navigate to="/login" replace />

  const isAdmin = user.role === 'Admin'
  const navItems = [
    ...ENTITIES.map((e) => ({ key: e.key, navKey: e.navKey, group: e.group, adminOnly: e.adminOnly })),
    { key: 'services', navKey: 'nav.services', group: 'nav.groupCatalog', adminOnly: false },
  ].filter((i) => !i.adminOnly || isAdmin)

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    cn('block rounded-md px-3 py-1.5 text-sm whitespace-nowrap', isActive ? 'bg-primary text-primary-foreground' : 'hover:bg-accent')

  // Render sidebar content with logo and navigation
  const SidebarContent = () => (
    <>
      {/* Clinic name and logo */}
      <div className="mb-4 px-2 flex items-center gap-2">
        {config?.logoUrl && <img src={config.logoUrl} alt="logo" className="h-8 w-8 rounded" />}
        <div className="text-sm font-semibold">{config?.name || t('common.appName')}</div>
      </div>
      <nav className="space-y-4">
        <NavLink to="/" end className={linkClass}>
          <span className="flex items-center gap-2"><LayoutDashboard size={15} /> {t('nav.dashboard')}</span>
        </NavLink>
        {GROUP_ORDER.map((group) => {
          const items = navItems.filter((i) => i.group === group)
          if (items.length === 0) return null
          return (
            <div key={group}>
              <p className="mb-1 px-2 text-xs font-semibold uppercase text-muted-foreground">{t(group)}</p>
              <div className="space-y-0.5">
                {items.map((i) => (
                  <NavLink key={i.key} to={`/${i.key}`} className={linkClass}>{t(i.navKey)}</NavLink>
                ))}
              </div>
            </div>
          )
        })}
        <div className="border-t pt-4">
          <NavLink to="/config" className={linkClass}>
            <span className="flex items-center gap-2"><Settings size={15} /> {t('nav.settings')}</span>
          </NavLink>
        </div>
      </nav>
    </>
  )

  return (
    <div className="flex min-h-screen flex-col lg:flex-row">
      {/* Collapsible sidebar navigation */}
      <aside className={cn(
        'fixed inset-y-0 left-0 z-50 w-60 transform border-r bg-card p-3 transition-transform lg:relative lg:translate-x-0',
        sidebarOpen ? 'translate-x-0' : '-translate-x-full'
      )}>
        {/* Close button for mobile */}
        <button
          onClick={() => setSidebarOpen(false)}
          className="mb-4 ml-auto block rounded-md p-1 lg:hidden"
          aria-label={t('common.closeSidebar')}
        >
          <X size={20} />
        </button>
        <SidebarContent />
      </aside>

      {/* Main content area */}
      <div className="flex w-full flex-1 flex-col">
        {/* Header with user info and controls */}
        <header className="flex items-center justify-between border-b bg-card px-3 py-3 lg:px-6">
          {/* Hamburger menu toggle for mobile */}
          <button
            onClick={() => setSidebarOpen(!sidebarOpen)}
            className="rounded-md p-1 lg:hidden"
            aria-label={t('common.toggleSidebar')}
          >
            <Menu size={20} />
          </button>
          <div className="hidden text-sm text-muted-foreground lg:block">{user.fullName}</div>
          <div className="flex items-center gap-2 lg:gap-3">
            {/* Mobile: show abbreviated user name */}
            <div className="block text-xs text-muted-foreground lg:hidden">{user.fullName}</div>
            <LanguageSwitcher />
            <Button variant="outline" size="sm" onClick={logout}>{t('common.logout')}</Button>
          </div>
        </header>

        {/* Overlay to close sidebar when clicking outside on mobile */}
        {sidebarOpen && (
          <div
            className="fixed inset-0 z-40 bg-black/50 lg:hidden"
            onClick={() => setSidebarOpen(false)}
          />
        )}

        {/* Page content */}
        <main className="flex-1 overflow-auto p-3 lg:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
