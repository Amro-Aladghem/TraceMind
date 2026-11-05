import { useState } from 'react';
import { Bell } from 'lucide-react';
import { Logo } from '@/components/layout/Logo';
import { Badge } from '@/components/ui/Badge';
import { IconButton } from '@/components/ui/IconButton';
import { NotificationDialog } from '@/components/notifications/NotificationDialog';
import { useNotifications } from '@/contexts/NotificationsContext';
import { cn } from '@/libs/cn';
import { NavLink } from '@/libs/router';

const linkClass =
  'rounded-lg px-3 py-2 text-sm font-medium text-muted transition-colors hover:text-ink';

export function AppHeader() {
  const { notifications } = useNotifications();
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  const notificationCount = notifications.length;

  return (
    <header className="sticky top-0 z-30 border-b border-line bg-canvas/85 backdrop-blur">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between gap-3 px-4 sm:px-6">
        <Logo />

        <nav className="flex items-center gap-1">
          <NavLink
            to="/"
            className={({ isActive }) =>
              cn(linkClass, isActive && 'bg-primary-soft text-primary-glow')
            }
          >
            Dashboard
          </NavLink>
          <NavLink
            to="/logs"
            className={({ isActive }) =>
              cn(linkClass, isActive && 'bg-primary-soft text-primary-glow')
            }
          >
            Logs
          </NavLink>

          <IconButton
            label="Notifications"
            onClick={() => setNotificationsOpen(true)}
          >
            <Bell size={20} />
            {notificationCount > 0 && (
              <Badge className="pointer-events-none absolute -right-0.5 -top-0.5">
                {notificationCount > 99 ? '99+' : notificationCount}
              </Badge>
            )}
          </IconButton>
        </nav>
      </div>

      <NotificationDialog
        open={notificationsOpen}
        onClose={() => setNotificationsOpen(false)}
      />
    </header>
  );
}
