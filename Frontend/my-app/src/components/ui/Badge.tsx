import type { ReactNode } from 'react';
import { cn } from '@/libs/cn';

export function Badge({ children, className }: { children: ReactNode; className?: string }) {
  return (
    <span
      className={cn(
        'inline-flex min-w-5 items-center justify-center rounded-full bg-primary px-1.5',
        'text-[11px] font-semibold leading-5 text-white',
        className,
      )}
    >
      {children}
    </span>
  );
}
