import { FileText, ArrowRight } from 'lucide-react';
import { DashboardCard } from '@/components/dashboard/DashboardCard';
import { LogLevelBadge } from '@/components/logs/LogLevelBadge';
import { EmptyState } from '@/components/ui/EmptyState';
import { ErrorState } from '@/components/ui/ErrorState';
import { LoadingState } from '@/components/ui/LoadingState';
import { useAsyncData } from '@/hooks/useAsyncData';
import { APPLICATION_ID } from '@/libs/config';
import { formatRelative } from '@/libs/format';
import { NavLink } from '@/libs/router';
import applicationService from '@/services/applicationService';

export function LatestLogsCard({ refreshKey }: { refreshKey: string }) {
  const logs = useAsyncData(
    () => applicationService.getLogs(APPLICATION_ID, { page: 1, pageSize: 6 }),
    [refreshKey],
  );

  return (
    <DashboardCard
      title="Latest logs"
      subtitle="Most recent entries across the application"
      icon={FileText}
      action={
        <NavLink
          to="/logs"
          className="inline-flex items-center gap-1 rounded-lg px-2 py-1 text-xs font-medium text-primary-glow transition-colors hover:bg-primary-soft"
        >
          View all
          <ArrowRight size={13} />
        </NavLink>
      }
    >
      {logs.loading && <LoadingState label="Loading logs…" />}
      {!logs.loading && logs.error && (
        <ErrorState message={logs.error} onRetry={logs.reload} />
      )}
      {!logs.loading && !logs.error && logs.data && logs.data.logs.length === 0 && (
        <EmptyState
          icon={<FileText size={20} />}
          title="No logs yet"
          description="Logs sent to TraceMind will show up here."
        />
      )}
      {!logs.loading && !logs.error && logs.data && logs.data.logs.length > 0 && (
        <ul className="divide-y divide-line">
          {logs.data.logs.map((log) => (
            <li key={log.id} className="flex flex-col gap-1.5 py-3 first:pt-0 last:pb-0">
              <div className="flex flex-wrap items-center gap-2">
                <LogLevelBadge level={log.level} />
                <span className="truncate text-sm text-ink">{log.message}</span>
              </div>
              <div className="flex flex-wrap items-center gap-2 text-[11px] text-muted">
                {log.requestMethod && (
                  <span className="font-mono text-primary-glow">{log.requestMethod}</span>
                )}
                {log.requestPath && <span className="font-mono">{log.requestPath}</span>}
                {log.statusCode !== null && log.statusCode !== undefined && (
                  <span
                    className={
                      log.statusCode >= 400 ? 'text-danger' : 'text-success'
                    }
                  >
                    {log.statusCode}
                  </span>
                )}
                <span aria-hidden="true">·</span>
                <span>{formatRelative(log.timestamp)}</span>
              </div>
            </li>
          ))}
        </ul>
      )}
    </DashboardCard>
  );
}
