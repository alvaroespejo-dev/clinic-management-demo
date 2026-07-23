import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { AuthProvider } from '@/lib/auth/AuthContext'
import { ConfigProvider } from '@/lib/config/ConfigContext'
import { ToastProvider } from '@/lib/toast/ToastContext'
import { ConfirmProvider } from '@/lib/dialog/useConfirmDialog'
import { ToastContainer } from '@/components/ui/Toast'
import { ConfirmDialogComponent } from '@/components/ui/ConfirmDialog'
import { ErrorBoundary } from '@/components/ErrorBoundary'
import { AppLayout } from '@/routes/AppLayout'
import { LoginPage } from '@/pages/LoginPage'
import { DashboardPage } from '@/pages/DashboardPage'
import { NotFoundPage } from '@/pages/NotFoundPage'
import { CrudPage } from '@/components/shared/CrudPage'
import { ServicesPage } from '@/features/ServicesPage'
import { ConfigPage } from '@/pages/ConfigPage'
import { ENTITIES } from '@/features/registry'

export default function App() {
  return (
    <ErrorBoundary>
      <AuthProvider>
        <ConfigProvider>
          <ToastProvider>
            <ConfirmProvider>
              <BrowserRouter>
                <Routes>
                  <Route path="/login" element={<LoginPage />} />
                  <Route path="/" element={<AppLayout />}>
                    <Route index element={<DashboardPage />} />
                    <Route path="services" element={<ServicesPage />} />
                    <Route path="config" element={<ConfigPage />} />
                    {ENTITIES.map((e) => (
                      <Route key={e.key} path={e.key} element={<CrudPage config={e} />} />
                    ))}
                    <Route path="*" element={<NotFoundPage />} />
                  </Route>
                </Routes>
              </BrowserRouter>
              <ToastContainer />
              <ConfirmDialogComponent />
            </ConfirmProvider>
          </ToastProvider>
        </ConfigProvider>
      </AuthProvider>
    </ErrorBoundary>
  )
}
