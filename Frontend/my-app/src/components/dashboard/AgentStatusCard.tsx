import { Activity, Clock3, Radar } from 'lucide-react';
import { DashboardCard } from '@/components/dashboard/DashboardCard';
import {
  getAgentPresentation,
  statusLabel,
} from '@/components/notifications/notificationPresentation';
import { useNotifications } from '@/contexts/NotificationsContext';
import { cn } from '@/libs/cn';
import { formatRelative } from '@/libs/format';

type Tone = 'running' | 'idle' | 'success' | 'danger' | 'warning';

const toneStyles: Record<Tone, { dot: string; text: string; ring: string }> = {
  running: {
    dot: 'bg-primary',
    text: 'text-primary-glow',
    ring: 'bg-primary-soft text-primary-glow animate-pulse-ring',
  },
  idle: {
    dot: 'bg-muted',
    text: 'text-muted',
    ring: 'bg-surface-3 text-muted',
  },
  success: {
    dot: 'bg-success',
    text: 'text-success',
    ring: 'bg-success-soft text-success',
  },
  danger: {
    dot: 'bg-danger',
    text: 'text-danger',
    ring: 'bg-danger-soft text-danger',
  },
  warning: {
    dot: 'bg-warning',
    text: 'text-warning',
    ring: 'bg-warning-soft text-warning',
  },
};

function resolveState(
  isAgentRunning: boolean,
  lastStatus: string | null,
): { tone: Tone; title: string; description: string } {
  if (isAgentRunning) {
    return {
      tone: 'running',
      title: lastStatus === 'Queued' ? 'Queued' : 'Agent is working',
      description:
        lastStatus === 'Queued'
          ? 'The request is waiting for the agent to pick it up.'
          : 'The agent is analysing logs and running its tools.',
    };
  }

  switch (lastStatus) {
    case 'Completed':
      return {
        tone: 'success',
        title: 'Completed',
        description: 'The agent finished its latest run successfully.',
      };
    case 'Failed':
      return {
        tone: 'danger',
        title: 'Failed',
        description: 'The agent stopped because of an error.',
      };
    case 'WaitingForUser':
      return {
        tone: 'warning',
        title: 'Waiting for you',
        description: 'The agent needs more input to continue.',
      };
    default:
      return {
        tone: 'idle',
        title: 'Idle',
        description: 'No agent activity yet. Trigger a run from the dashboard.',
      };
  }
}

export function AgentStatusCard() {
  const { lastNotification, isAgentRunning, notifications } = useNotifications();
  const lastStatus = lastNotification?.notificationStatus ?? null;
  const state = resolveState(isAgentRunning, lastStatus);
  const tone = toneStyles[state.tone];
  const agent = lastNotification ? getAgentPresentation(lastNotification.agentType) : null;

  const runningCount = notifications.filter(
    (notification) => notification.notificationStatus === 'Running',
  ).length;

  const queuedCount = notifications.filter(
    (notification) => notification.notificationStatus === 'Queued',
  ).length;

  return (
    <DashboardCard
      title="Agent status"
      subtitle="Based on the latest notification state"
      icon={Activity}
    >
      <div className="flex flex-col gap-5">
        <div className="flex items-center gap-4">
          <span
            className={cn(
              'flex h-14 w-14 shrink-0 items-center justify-center rounded-full',
              tone.ring,
            )}
          >
            <Radar size={24} />
          </span>
          <div className="min-w-0">
            <div className="flex items-center gap-2">
              <span className={cn('h-2.5 w-2.5 rounded-full', tone.dot)} />
              <p className={cn('text-lg font-semibold', tone.text)}>{state.title}</p>
            </div>
            <p className="mt-1 text-sm text-muted">{state.description}</p>
          </div>
        </div>

        <div className="grid grid-cols-3 gap-2">
          <div className="rounded-xl border border-line bg-surface-2 px-3 py-2">
            <p className="text-[11px] uppercase tracking-wide text-muted">Notifications</p>
            <p className="mt-1 text-lg font-semibold text-ink">{notifications.length}</p>
          </div>
          <div className="rounded-xl border border-line bg-surface-2 px-3 py-2">
            <p className="text-[11px] uppercase tracking-wide text-muted">Running</p>
            <p className="mt-1 text-lg font-semibold text-ink">{runningCount}</p>
          </div>
          <div className="rounded-xl border border-line bg-surface-2 px-3 py-2">
            <p className="text-[11px] uppercase tracking-wide text-muted">Requests</p>
            <p className="mt-1 text-lg font-semibold text-ink">{queuedCount}</p>
            <p className="text-[10px] text-muted">Queued</p>
          </div>
        </div>

        {lastNotification ? (
          <div className="rounded-xl border border-line bg-surface-2 p-3">
            <div className="flex flex-wrap items-center gap-2 text-xs text-muted">
              <Clock3 size={13} />
              <span>Last update {formatRelative(lastNotification.updatedAt)}</span>
              {agent && (
                <>
                  <span aria-hidden="true">·</span>
                  <span>{agent.label}</span>
                </>
              )}
              {lastStatus && (
                <>
                  <span aria-hidden="true">·</span>
                  <span>{statusLabel[lastStatus as keyof typeof statusLabel] ?? lastStatus}</span>
                </>
              )}
            </div>
            {lastNotification.text && (
              <p className="mt-2 line-clamp-3 text-sm text-ink">
                {lastNotification.text}
              </p>
            )}
          </div>
        ) : (
          <p className="text-sm text-muted">No notification has been received yet.</p>
        )}
      </div>
    </DashboardCard>
  );
}
