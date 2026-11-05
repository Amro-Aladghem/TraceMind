import { useCallback, useEffect, useRef, useState } from 'react';
import type { DependencyList } from 'react';
import type { ApiResponse } from '@/types/api';

export interface AsyncData<T> {
  data: T | null;
  loading: boolean;
  error: string | null;
  reload: () => void;
}

export function useAsyncData<T>(
  loader: () => Promise<ApiResponse<T>>,
  deps: DependencyList = [],
): AsyncData<T> {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [nonce, setNonce] = useState(0);
  const loaderRef = useRef(loader);

  useEffect(() => {
    loaderRef.current = loader;
  });

  const reload = useCallback(() => {
    setLoading(true);
    setError(null);
    setNonce((value) => value + 1);
  }, []);

  useEffect(() => {
    let active = true;
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setLoading(true);
    setError(null);
    loaderRef
      .current()
      .then((response) => {
        if (!active) return;
        if (response.isSuccess) {
          setData(response.data);
          setError(null);
        } else {
          setError(response.error);
        }
      })
      .catch(() => {
        if (active) setError('Unexpected error occurred');
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [nonce, ...deps]);

  return { data, loading, error, reload };
}
