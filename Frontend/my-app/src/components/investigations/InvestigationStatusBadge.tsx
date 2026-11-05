import type { InvestigationStatus } from '@/types/enums';
import { cn } from '@/libs/cn';

const statusStyles: Record<InvestigationStatus, string> = {
  Pending: 'border-line bg-surface-3 text-muted',
  Running: 'border-primary/40 bg-primary-soft text-primary-glow',
  Completed: 'border-success/30 bg-success-soft text-success',
  Failed: 'border-danger/30 bg-danger-soft text-danger',
};

export function InvestigationStatusBadge({ status }: { status: InvestigationStatus }) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full border px-2 py-0.5 text-[11px] font-semibold',
        statusStyles[status] ?? statusStyles.Pending,
      )}
    >
      {status}
    </span>
  );
}
