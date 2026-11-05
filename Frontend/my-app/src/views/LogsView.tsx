import { useState } from 'react';
import { ChevronLeft, ChevronRight, Filter, RotateCcw } from 'lucide-react';
import { LogsTable } from '@/components/logs/LogsTable';
import { Button } from '@/components/ui/Button';
import { useAsyncData } from '@/hooks/useAsyncData';
import { APPLICATION_ID, LOGS_PAGE_SIZE } from '@/libs/config';
import { formatNumber } from '@/libs/format';
import applicationService from '@/services/applicationService';
import { logLevels } from '@/types/enums';
import type { LogLevel } from '@/types/enums';
import type { GetLogsParams } from '@/types/log';

interface FilterForm {
  level: LogLevel | '';
  requestPath: string;
  requestMethod: string;
  statusCode: string;
  traceId: string;
  from: string;
  to: string;
  pageSize: number;
}

const emptyForm: FilterForm = {
  level: '',
  requestPath: '',
  requestMethod: '',
  statusCode: '',
  traceId: '',
  from: '',
  to: '',
  pageSize: LOGS_PAGE_SIZE,
};

const inputClass =
  'h-10 w-full rounded-xl border border-line bg-surface-2 px-3 text-sm text-ink ' +
  'placeholder:text-muted/70 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary';

const labelClass = 'mb-1.5 block text-[11px] font-medium uppercase tracking-wide text-muted';

function toIso(value: string): string | undefined {
  if (!value) return undefined;
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return undefined;
  return date.toISOString();
}

export function LogsView() {
  const [form, setForm] = useState<FilterForm>(emptyForm);
  const [applied, setApplied] = useState<GetLogsParams>({
    page: 1,
    pageSize: LOGS_PAGE_SIZE,
  });

  const logs = useAsyncData(
    () => applicationService.getLogs(APPLICATION_ID, applied),
    [applied],
  );

  const totalCount = logs.data?.totalCount ?? 0;
  const pageSize = applied.pageSize ?? LOGS_PAGE_SIZE;
  const page = applied.page ?? 1;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  const applyFilters = () => {
    const statusCode = Number(form.statusCode);
    setApplied({
      page: 1,
      pageSize: form.pageSize,
      level: form.level || undefined,
      requestPath: form.requestPath.trim() || undefined,
      requestMethod: form.requestMethod || undefined,
      statusCode: Number.isFinite(statusCode) && form.statusCode.trim() !== '' ? statusCode : undefined,
      traceId: form.traceId.trim() || undefined,
      from: toIso(form.from),
      to: toIso(form.to),
    });
  };

  const resetFilters = () => {
    setForm(emptyForm);
    setApplied({ page: 1, pageSize: LOGS_PAGE_SIZE });
  };

  const goToPage = (nextPage: number) => {
    setApplied((previous) => ({
      ...previous,
      page: Math.min(Math.max(1, nextPage), totalPages),
    }));
  };

  return (
    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 sm:py-10">
      <div className="mb-6 flex flex-wrap items-end justify-between gap-4">
        <div>
          <p className="text-[11px] font-semibold uppercase tracking-[0.2em] text-primary-glow">
            Observability
          </p>
          <h1 className="mt-1 text-2xl font-semibold tracking-tight text-ink sm:text-3xl">
            Logs
          </h1>
          <p className="mt-1 max-w-xl text-sm text-muted">
            Filter the raw log stream by level, request, status code or trace id.
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={logs.reload}>
          <RotateCcw size={15} />
          Refresh
        </Button>
      </div>

      <section className="mb-4 rounded-2xl border border-line bg-surface p-4 sm:p-5">
        <div className="mb-4 flex items-center gap-2 text-sm font-semibold text-ink">
          <Filter size={16} className="text-primary-glow" />
          Filters
        </div>

        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <div>
            <label className={labelClass} htmlFor="level">
              Level
            </label>
            <select
              id="level"
              className={inputClass}
              value={form.level}
              onChange={(event) =>
                setForm((previous) => ({
                  ...previous,
                  level: event.target.value as LogLevel | '',
                }))
              }
            >
              <option value="">All levels</option>
              {logLevels.map((level) => (
                <option key={level} value={level}>
                  {level}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className={labelClass} htmlFor="requestMethod">
              Method
            </label>
            <select
              id="requestMethod"
              className={inputClass}
              value={form.requestMethod}
              onChange={(event) =>
                setForm((previous) => ({
                  ...previous,
                  requestMethod: event.target.value,
                }))
              }
            >
              <option value="">All methods</option>
              {['GET', 'POST', 'PUT', 'PATCH', 'DELETE'].map((method) => (
                <option key={method} value={method}>
                  {method}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className={labelClass} htmlFor="requestPath">
              Request path
            </label>
            <input
              id="requestPath"
              className={inputClass}
              placeholder="/api/payments"
              value={form.requestPath}
              onChange={(event) =>
                setForm((previous) => ({ ...previous, requestPath: event.target.value }))
              }
            />
          </div>

          <div>
            <label className={labelClass} htmlFor="statusCode">
              Status code
            </label>
            <input
              id="statusCode"
              className={inputClass}
              inputMode="numeric"
              placeholder="500"
              value={form.statusCode}
              onChange={(event) =>
                setForm((previous) => ({ ...previous, statusCode: event.target.value }))
              }
            />
          </div>

          <div>
            <label className={labelClass} htmlFor="traceId">
              Trace id
            </label>
            <input
              id="traceId"
              className={inputClass}
              placeholder="4f2a9c1e…"
              value={form.traceId}
              onChange={(event) =>
                setForm((previous) => ({ ...previous, traceId: event.target.value }))
              }
            />
          </div>

          <div>
            <label className={labelClass} htmlFor="from">
              From
            </label>
            <input
              id="from"
              type="datetime-local"
              className={inputClass}
              value={form.from}
              onChange={(event) =>
                setForm((previous) => ({ ...previous, from: event.target.value }))
              }
            />
          </div>

          <div>
            <label className={labelClass} htmlFor="to">
              To
            </label>
            <input
              id="to"
              type="datetime-local"
              className={inputClass}
              value={form.to}
              onChange={(event) =>
                setForm((previous) => ({ ...previous, to: event.target.value }))
              }
            />
          </div>

          <div>
            <label className={labelClass} htmlFor="pageSize">
              Page size
            </label>
            <select
              id="pageSize"
              className={inputClass}
              value={form.pageSize}
              onChange={(event) =>
                setForm((previous) => ({
                  ...previous,
                  pageSize: Number(event.target.value),
                }))
              }
            >
              {[10, 20, 50, 100].map((size) => (
                <option key={size} value={size}>
                  {size}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="mt-4 flex flex-wrap items-center gap-2">
          <Button size="sm" onClick={applyFilters}>
            Apply filters
          </Button>
          <Button variant="ghost" size="sm" onClick={resetFilters}>
            <RotateCcw size={14} />
            Reset
          </Button>
        </div>
      </section>

      <section className="overflow-hidden rounded-2xl border border-line bg-surface">
        <div className="flex flex-wrap items-center justify-between gap-2 border-b border-line px-4 py-3">
          <p className="text-sm text-muted">
            <span className="font-semibold text-ink">{formatNumber(totalCount)}</span>{' '}
            log{totalCount === 1 ? '' : 's'} found
          </p>
          <p className="text-xs text-muted">
            Page {page} of {totalPages}
          </p>
        </div>

        <LogsTable
          logs={logs.data?.logs ?? []}
          loading={logs.loading}
          error={logs.error}
          onRetry={logs.reload}
        />

        <div className="flex flex-wrap items-center justify-between gap-3 border-t border-line px-4 py-3">
          <p className="text-[11px] text-muted">
            Page size {pageSize} · newest first
          </p>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={page <= 1 || logs.loading}
              onClick={() => goToPage(page - 1)}
            >
              <ChevronLeft size={14} />
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={page >= totalPages || logs.loading}
              onClick={() => goToPage(page + 1)}
            >
              Next
              <ChevronRight size={14} />
            </Button>
          </div>
        </div>
      </section>
    </div>
  );
}
