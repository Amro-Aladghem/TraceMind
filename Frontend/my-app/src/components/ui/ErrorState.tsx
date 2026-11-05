import { AlertTriangle } from 'lucide-react';
import { Button } from '@/components/ui/Button';

interface ErrorStateProps {
  message: string;
  onRetry: () => void;
}

export function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 rounded-2xl border border-line bg-surface px-6 py-12 text-center">
      <span className="flex h-12 w-12 items-center justify-center rounded-full bg-danger-soft text-danger">
        <AlertTriangle size={22} />
      </span>
      <h3 className="text-base font-semibold text-ink">Something went wrong</h3>
      <p className="max-w-sm text-sm text-muted">{message}</p>
      <Button variant="secondary" size="sm" onClick={onRetry}>
        Try again
      </Button>
    </div>
  );
}
