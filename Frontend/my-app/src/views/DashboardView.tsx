import { useMemo, useState } from 'react';
import { RefreshCw } from 'lucide-react';
import { AgentStatusCard } from '@/components/dashboard/AgentStatusCard';
import { InvestigationCard } from '@/components/dashboard/InvestigationCard';
import { LatestLogsCard } from '@/components/dashboard/LatestLogsCard';
import { NotificationsCard } from '@/components/dashboard/NotificationsCard';
import { Button } from '@/components/ui/Button';
import { useNotifications } from '@/contexts/NotificationsContext';
import { useDefaultAgentCall } from '@/hooks/useDefaultAgentCall';
import { APPLICATION_ID } from '@/libs/config';

export function DashboardView() {
  //useDefaultAgentCall();
  const { lastNotification, reload } = useNotifications();
  const [manualRefresh, setManualRefresh] = useState(0);

  const notificationKey = lastNotification
    ? `${lastNotification.id}:${lastNotification.updatedAt}:${lastNotification.notificationStatus}`
    : 'none';

  const refreshKey = useMemo(
    () => `${notificationKey}:${manualRefresh}`,
    [notificationKey, manualRefresh],
  );

  const handleRefresh = () => {
    setManualRefresh((value) => value + 1);
    reload();
  };

  return (
    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 sm:py-10">
      <div className="mb-6 flex flex-wrap items-end justify-between gap-4">
        <div>
          <p className="text-[11px] font-semibold uppercase tracking-[0.2em] text-primary-glow">
            TraceMind
          </p>
          <h1 className="mt-1 text-2xl font-semibold tracking-tight text-ink sm:text-3xl">
            Dashboard
          </h1>
          <p className="mt-1 max-w-xl text-sm text-muted">
            AI investigations, recent logs and live agent activity for your application.
          </p>
          <p className="mt-2 inline-flex items-center gap-2 rounded-full border border-line bg-surface px-2.5 py-1 font-mono text-[11px] text-muted">
            <span className="h-1.5 w-1.5 rounded-full bg-primary" />
            {APPLICATION_ID}
          </p>
        </div>

        <Button variant="outline" size="sm" onClick={handleRefresh}>
          <RefreshCw size={15} />
          Refresh
        </Button>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <InvestigationCard refreshKey={refreshKey} />
        </div>
        <div>
          <AgentStatusCard />
        </div>
        <div className="lg:col-span-2">
          <LatestLogsCard refreshKey={refreshKey} />
        </div>
        <div>
          <NotificationsCard />
        </div>
      </div>
    </div>
  );
}
