/* eslint-disable react-refresh/only-export-components */
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from 'react';
import type { ReactNode } from 'react';
import { APPLICATION_ID, NOTIFICATIONS_POLL_INTERVAL_MS } from '@/libs/config';
import applicationService from '@/services/applicationService';
import { terminalNotificationStatuses } from '@/types/enums';
import type { NotificationDto } from '@/types/notification';

interface NotificationsContextValue {
  notifications: NotificationDto[];
  lastNotification: NotificationDto | null;
  isAgentRunning: boolean;
  loading: boolean;
  error: string | null;
  reload: () => void;
}

const NotificationsContext = createContext<NotificationsContextValue | null>(null);

function lastItem(notifications: NotificationDto[]): NotificationDto | null {
  // The API returns notifications ordered by UpdatedAt ascending.
  return notifications.length > 0 ? notifications[notifications.length - 1] : null;
}

export function NotificationsProvider({ children }: { children: ReactNode }) {
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const inFlightRef = useRef(false);
  const loadedOnceRef = useRef(false);

  const load = useCallback(async () => {
    if (inFlightRef.current) return;
    inFlightRef.current = true;

    try {
      const result = await applicationService.getNotifications(APPLICATION_ID);
      if (result.isSuccess) {
        setNotifications(result.data);
        setError(null);
      } else {
        setError(result.error);
      }
    } catch {
      setError('Unexpected error occurred');
    } finally {
      inFlightRef.current = false;
      if (!loadedOnceRef.current) {
        loadedOnceRef.current = true;
        setLoading(false);
      }
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void load();
    const interval = window.setInterval(() => {
      void load();
    }, NOTIFICATIONS_POLL_INTERVAL_MS);

    return () => window.clearInterval(interval);
  }, [load]);

  const lastNotification = useMemo(() => lastItem(notifications), [notifications]);

  const isAgentRunning = useMemo(() => {
    if (!lastNotification) return false;
    return !terminalNotificationStatuses.includes(lastNotification.notificationStatus);
  }, [lastNotification]);

  const reload = useCallback(() => {
    void load();
  }, [load]);

  const value = useMemo<NotificationsContextValue>(
    () => ({
      notifications,
      lastNotification,
      isAgentRunning,
      loading,
      error,
      reload,
    }),
    [notifications, lastNotification, isAgentRunning, loading, error, reload],
  );

  return (
    <NotificationsContext.Provider value={value}>
      {children}
    </NotificationsContext.Provider>
  );
}

export function useNotifications(): NotificationsContextValue {
  const ctx = useContext(NotificationsContext);
  if (!ctx) throw new Error('useNotifications must be used within a NotificationsProvider');
  return ctx;
}
