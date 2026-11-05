import type { ReactNode } from 'react';
import type { LucideIcon } from 'lucide-react';
import { cn } from '@/libs/cn';

interface DashboardCardProps {
  title: string;
  subtitle?: string;
  icon: LucideIcon;
  action?: ReactNode;
  children: ReactNode;
  className?: string;
}

export function DashboardCard({
  title,
  subtitle,
  icon: Icon,
  action,
  children,
  className,
}: DashboardCardProps) {
  return (
    <section
      className={cn(
        'flex min-h-0 flex-col overflow-hidden rounded-2xl border border-line bg-surface',
        className,
      )}
    >
      <header className="flex items-center justify-between gap-3 border-b border-line px-5 py-4">
        <div className="flex min-w-0 items-center gap-3">
          <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl bg-primary-soft text-primary-glow">
            <Icon size={18} />
          </span>
          <div className="min-w-0">
            <h2 className="truncate text-sm font-semibold text-ink">{title}</h2>
            {subtitle && <p className="truncate text-xs text-muted">{subtitle}</p>}
          </div>
        </div>
        {action}
      </header>
      <div className="flex-1 p-5">{children}</div>
    </section>
  );
}
