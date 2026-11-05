import { useState } from 'react';
import { Bell, BellOff } from 'lucide-react';
import { DashboardCard } from '@/components/dashboard/DashboardCard';
import { NotificationDialog } from '@/components/notifications/NotificationDialog';
import { NotificationItem } from '@/components/notifications/NotificationItem';
import { EmptyState } from '@/components/ui/EmptyState';
import { ErrorState } from '@/components/ui/ErrorState';
import { LoadingState } from '@/components/ui/LoadingState';
import { useNotifications } from '@/contexts/NotificationsContext';

export function NotificationsCard() {
  const { notifications, loading, error, reload } = useNotifications();
  const [open, setOpen] = useState(false);
  const latest = notifications.slice(-5).reverse();

  return (
    <>
      <DashboardCard
        title="Notifications"
        subtitle="Latest agent status updates"
        icon={Bell}
        action={
          notifications.length > 0 && (
            <button
              type="button"
              onClick={() => setOpen(true)}
              className="rounded-lg px-2 py-1 text-xs font-medium text-primary-glow transition-colors hover:bg-primary-soft"
            >
              View all
            </button>
          )
        }
      >
        {loading && notifications.length === 0 && (
          <LoadingState label="Loading notifications…" />
        )}
        {!loading && error && notifications.length === 0 && (
          <ErrorState message={error} onRetry={reload} />
        )}
        {!loading && !error && notifications.length === 0 && (
          <EmptyState
            icon={<BellOff size={20} />}
            title="No notifications yet"
            description="Agent activity will appear here."
          />
        )}
        {latest.length > 0 && (
          <ul className="flex flex-col gap-2">
            {latest.map((notification, index) => (
              <li key={notification.id}>
                <NotificationItem
                  notification={notification}
                  isLast={index === 0}
                />
              </li>
            ))}
          </ul>
        )}
      </DashboardCard>

      <NotificationDialog open={open} onClose={() => setOpen(false)} />
    </>
  );
}
