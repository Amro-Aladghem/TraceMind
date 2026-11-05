import type { ButtonHTMLAttributes, ReactNode } from 'react';
import { cn } from '@/libs/cn';

interface IconButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  label: string;
  children: ReactNode;
}

export function IconButton({
  label,
  children,
  className,
  type = 'button',
  ...props
}: IconButtonProps) {
  return (
    <button
      type={type}
      aria-label={label}
      title={label}
      className={cn(
        'relative inline-flex h-10 w-10 items-center justify-center rounded-full text-ink transition-colors',
        'hover:bg-primary-soft focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary',
        'disabled:cursor-not-allowed disabled:opacity-50',
        className,
      )}
      {...props}
    >
      {children}
    </button>
  );
}
