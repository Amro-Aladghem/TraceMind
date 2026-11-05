import { Radar } from 'lucide-react';
import { NavLink } from '@/libs/router';

export function Logo() {
  return (
    <NavLink to="/" className="flex items-center gap-2" aria-label="TraceMind home">
      <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-primary text-white shadow-lg shadow-primary/30">
        <Radar size={20} />
      </span>
      <span className="text-lg font-semibold tracking-tight text-ink">
        Trace<span className="text-primary-glow">Mind</span>
      </span>
    </NavLink>
  );
}
