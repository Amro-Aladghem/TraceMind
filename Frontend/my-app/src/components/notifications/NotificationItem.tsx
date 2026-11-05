import { cn } from '@/libs/cn';
import { formatRelative } from '@/libs/format';
import {
  getAgentPresentation,
  statusLabel,
  statusPresentation,
} from '@/components/notifications/notificationPresentation';
import type { NotificationDto } from '@/types/notification';
import type { NotificationStatus } from '@/types/enums';

interface NotificationItemProps {
  notification: NotificationDto;
  isLast: boolean;
}

export function NotificationItem({ notification, isLast }: NotificationItemProps) {
  const agent = getAgentPresentation(notification.agentType);
  const presentedStatus: NotificationStatus =
    notification.notificationStatus === 'Running' && !isLast
      ? 'Completed'
      : notification.notificationStatus;
  const status = statusPresentation[presentedStatus] ?? statusPresentation.Queued;
  const AgentIcon = agent.Icon;
  const StatusIcon = status.Icon;

  return (
    <div className="flex w-full gap-3 rounded-xl border border-line bg-surface-2 p-3">
      <span
        className={cn(
          'mt-0.5 flex h-10 w-10 shrink-0 items-center justify-center rounded-full',
          agent.tint,
        )}
      >
        <AgentIcon size={18} />
      </span>
      <div className="min-w-0 flex-1">
        {notification.text && (
          <p className="whitespace-pre-wrap break-words text-sm text-ink">
            {notification.text}
          </p>
        )}
        <div className="mt-2 flex flex-wrap items-center gap-2 text-xs text-muted">
          <StatusIcon
            size={13}
            className={cn(status.tint, status.spin && 'animate-spin')}
            aria-hidden="true"
          />
          <span>{agent.label}</span>
          <span aria-hidden="true">·</span>
          <span>{statusLabel[presentedStatus] ?? presentedStatus}</span>
          <span aria-hidden="true">·</span>
          <span>{formatRelative(notification.updatedAt)}</span>
        </div>
      </div>
    </div>
  );
}
