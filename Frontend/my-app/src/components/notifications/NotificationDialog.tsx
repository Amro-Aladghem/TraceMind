import { BellOff } from 'lucide-react';
import { useNotifications } from '@/contexts/NotificationsContext';
import { Dialog } from '@/components/ui/Dialog';
import { EmptyState } from '@/components/ui/EmptyState';
import { NotificationItem } from '@/components/notifications/NotificationItem';

interface NotificationDialogProps {
  open: boolean;
  onClose: () => void;
}

export function NotificationDialog({ open, onClose }: NotificationDialogProps) {
  const { notifications } = useNotifications();

  return (
    <Dialog open={open} onClose={onClose} title="Notifications" size="sm">
      {notifications.length === 0 ? (
        <EmptyState
          icon={<BellOff size={20} />}
          title="No notifications yet"
          description="Agent activity will appear here."
        />
      ) : (
        <ul className="flex flex-col gap-2">
          {notifications.map((notification, index) => (
            <li key={notification.id}>
              <NotificationItem
                notification={notification}
                isLast={index === notifications.length - 1}
              />
            </li>
          ))}
        </ul>
      )}
    </Dialog>
  );
}
