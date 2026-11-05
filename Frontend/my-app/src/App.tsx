import { AppHeader } from '@/components/layout/AppHeader';
import { NotificationsProvider } from '@/contexts/NotificationsContext';
import { RouterProvider, useRouter } from '@/libs/router';
import { DashboardView } from '@/views/DashboardView';
import { LogsView } from '@/views/LogsView';

function CurrentView() {
  const { path } = useRouter();

  if (path === '/logs') return <LogsView />;

  return <DashboardView />;
}

function AppShell() {
  return (
    <div className="flex min-h-screen flex-col bg-canvas">
      <AppHeader />
      <main className="flex-1">
        <CurrentView />
      </main>
    </div>
  );
}

function App() {
  return (
    <RouterProvider>
      <NotificationsProvider>
        <AppShell />
      </NotificationsProvider>
    </RouterProvider>
  );
}

export default App;
