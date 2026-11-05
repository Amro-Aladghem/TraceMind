import { useEffect, useRef } from 'react';
import type { ReactNode } from 'react';
import { createPortal } from 'react-dom';
import { X } from 'lucide-react';
import { cn } from '@/libs/cn';
import { IconButton } from '@/components/ui/IconButton';

type DialogSize = 'sm' | 'md' | 'lg' | 'xl';

interface DialogProps {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  footer?: ReactNode;
  size?: DialogSize;
}

const sizeClasses: Record<DialogSize, string> = {
  sm: 'sm:max-w-md',
  md: 'sm:max-w-lg',
  lg: 'sm:max-w-2xl',
  xl: 'sm:max-w-4xl',
};

export function Dialog({
  open,
  onClose,
  title,
  children,
  footer,
  size = 'md',
}: DialogProps) {
  const panelRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', onKeyDown);
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    panelRef.current?.focus();
    return () => {
      document.removeEventListener('keydown', onKeyDown);
      document.body.style.overflow = previousOverflow;
    };
  }, [open, onClose]);

  if (!open) return null;

  return createPortal(
    <div
      className="fixed inset-0 z-50 flex items-end justify-center bg-black/70 backdrop-blur-sm sm:items-center sm:p-4"
      onMouseDown={onClose}
      role="presentation"
    >
      <div
        ref={panelRef}
        role="dialog"
        aria-modal="true"
        aria-label={title}
        tabIndex={-1}
        onMouseDown={(event) => event.stopPropagation()}
        className={cn(
          'flex max-h-[92vh] w-full flex-col border border-line bg-surface shadow-2xl outline-none',
          'animate-slide-up rounded-t-2xl sm:animate-pop-in sm:rounded-2xl',
          sizeClasses[size],
        )}
      >
        <header className="flex items-center justify-between gap-3 border-b border-line px-5 py-4">
          <h2 className="text-lg font-semibold text-ink">{title}</h2>
          <IconButton label="Close" onClick={onClose}>
            <X size={18} />
          </IconButton>
        </header>
        <div className="flex-1 overflow-y-auto px-5 py-4 scrollbar-thin">{children}</div>
        {footer && <footer className="border-t border-line px-5 py-4">{footer}</footer>}
      </div>
    </div>,
    document.body,
  );
}
