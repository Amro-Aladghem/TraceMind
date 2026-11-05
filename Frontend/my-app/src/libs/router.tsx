/* eslint-disable react-refresh/only-export-components */
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react';
import type { MouseEvent, ReactNode } from 'react';

interface RouterContextValue {
  path: string;
  navigate: (to: string) => void;
}

const RouterContext = createContext<RouterContextValue | null>(null);

export function RouterProvider({ children }: { children: ReactNode }) {
  const [path, setPath] = useState<string>(() => window.location.pathname || '/');

  useEffect(() => {
    const onPopState = () => setPath(window.location.pathname || '/');
    window.addEventListener('popstate', onPopState);
    return () => window.removeEventListener('popstate', onPopState);
  }, []);

  const navigate = useCallback((to: string) => {
    if (to === (window.location.pathname || '/')) return;
    window.history.pushState({}, '', to);
    setPath(to);
    window.scrollTo({ top: 0, behavior: 'auto' });
  }, []);

  const value = useMemo(() => ({ path, navigate }), [path, navigate]);

  return <RouterContext.Provider value={value}>{children}</RouterContext.Provider>;
}

export function useRouter(): RouterContextValue {
  const ctx = useContext(RouterContext);
  if (!ctx) throw new Error('useRouter must be used within a RouterProvider');
  return ctx;
}

export function useNavigate(): (to: string) => void {
  return useRouter().navigate;
}

/** Matches the current path against a simple pattern such as `/logs`. */
export function matchPath(pattern: string, path: string): Record<string, string> | null {
  const patternParts = pattern.split('/').filter(Boolean);
  const pathParts = path.split('/').filter(Boolean);

  if (patternParts.length !== pathParts.length) return null;

  const params: Record<string, string> = {};

  for (let index = 0; index < patternParts.length; index += 1) {
    const patternPart = patternParts[index];
    const pathPart = pathParts[index];

    if (patternPart.startsWith(':')) {
      params[patternPart.slice(1)] = decodeURIComponent(pathPart);
      continue;
    }

    if (patternPart !== pathPart) return null;
  }

  return params;
}

interface NavLinkProps {
  to: string;
  children: ReactNode;
  className?: string | ((state: { isActive: boolean }) => string);
  onNavigate?: () => void;
}

export function NavLink({ to, children, className, onNavigate }: NavLinkProps) {
  const { path, navigate } = useRouter();
  const isActive = path === to;

  const handleClick = (event: MouseEvent<HTMLAnchorElement>) => {
    event.preventDefault();
    navigate(to);
    onNavigate?.();
  };

  return (
    <a
      href={to}
      onClick={handleClick}
      aria-current={isActive ? 'page' : undefined}
      className={typeof className === 'function' ? className({ isActive }) : className}
    >
      {children}
    </a>
  );
}
