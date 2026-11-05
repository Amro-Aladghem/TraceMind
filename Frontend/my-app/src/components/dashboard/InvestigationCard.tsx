import { useState } from 'react';
import { BrainCircuit, ChevronRight, Sparkles } from 'lucide-react';
import { DashboardCard } from '@/components/dashboard/DashboardCard';
import { InvestigationStatusBadge } from '@/components/investigations/InvestigationStatusBadge';
import { Dialog } from '@/components/ui/Dialog';
import { EmptyState } from '@/components/ui/EmptyState';
import { ErrorState } from '@/components/ui/ErrorState';
import { LoadingState } from '@/components/ui/LoadingState';
import { useAsyncData } from '@/hooks/useAsyncData';
import { APPLICATION_ID } from '@/libs/config';
import { formatDateTime, formatRelative } from '@/libs/format';
import applicationService from '@/services/applicationService';
import type { PreviousInvestigationDto } from '@/types/investigation';

function formatJson(value: unknown): string {
  if (value === null || value === undefined) return '';
  if (typeof value === 'string') return value;
  try {
    return JSON.stringify(value, null, 2);
  } catch {
    return String(value);
  }
}

function RecommendationList({ value }: { value: unknown }) {
  if (value === null || value === undefined) {
    return <p className="text-sm text-muted">No recommendations were recorded.</p>;
  }

  if (Array.isArray(value)) {
    return (
      <ul className="space-y-2">
        {value.map((item, index) => (
          <li
            key={index}
            className="rounded-lg border border-line bg-surface-2 px-3 py-2 text-sm text-ink"
          >
            {typeof item === 'string' ? item : formatJson(item)}
          </li>
        ))}
      </ul>
    );
  }

  if (typeof value === 'object') {
    return (
      <dl className="space-y-1.5">
        {Object.entries(value as Record<string, unknown>).map(([key, item]) => (
          <div key={key} className="flex flex-wrap gap-1 text-sm">
            <dt className="text-muted">{key}:</dt>
            <dd className="text-ink">{typeof item === 'string' ? item : formatJson(item)}</dd>
          </div>
        ))}
      </dl>
    );
  }

  return <p className="text-sm text-ink">{String(value)}</p>;
}

function InvestigationDialog({
  investigationId,
  onClose,
}: {
  investigationId: string;
  onClose: () => void;
}) {
  const details = useAsyncData(
    () => applicationService.getInvestigationById(APPLICATION_ID, investigationId),
    [investigationId],
  );

  return (
    <Dialog open onClose={onClose} title="Investigation details" size="lg">
      {details.loading && <LoadingState label="Loading investigation…" />}
      {!details.loading && details.error && (
        <ErrorState message={details.error} onRetry={details.reload} />
      )}
      {!details.loading && !details.error && details.data && (
        <div className="space-y-5">
          <div className="flex flex-wrap items-start justify-between gap-3">
            <div>
              <h3 className="text-lg font-semibold text-ink">{details.data.title}</h3>
              <p className="mt-1 text-xs text-muted">
                Started {formatDateTime(details.data.startedAt)}
                {details.data.completedAt
                  ? ` · Completed ${formatDateTime(details.data.completedAt)}`
                  : ''}
              </p>
            </div>
            <InvestigationStatusBadge status={details.data.status} />
          </div>

          <div className="grid gap-3 sm:grid-cols-3">
            <div className="rounded-xl border border-line bg-surface-2 px-3 py-2">
              <p className="text-[11px] uppercase tracking-wide text-muted">Model</p>
              <p className="mt-1 text-sm text-ink">{details.data.modelName ?? '—'}</p>
            </div>
            <div className="rounded-xl border border-line bg-surface-2 px-3 py-2">
              <p className="text-[11px] uppercase tracking-wide text-muted">Confidence</p>
              <p className="mt-1 text-sm text-ink">
                {typeof details.data.confidence === 'number'
                  ? `${Math.round(details.data.confidence * 100)}%`
                  : '—'}
              </p>
            </div>
            <div className="rounded-xl border border-line bg-surface-2 px-3 py-2">
              <p className="text-[11px] uppercase tracking-wide text-muted">Investigation ID</p>
              <p className="mt-1 break-all font-mono text-xs text-ink">
                {details.data.id}
              </p>
            </div>
          </div>

          <div>
            <h4 className="mb-2 text-sm font-semibold text-ink">Summary</h4>
            <p className="whitespace-pre-wrap break-words text-sm text-muted">
              {details.data.summary ?? 'No summary was recorded.'}
            </p>
          </div>

          <div>
            <h4 className="mb-2 text-sm font-semibold text-ink">Root cause</h4>
            <p className="whitespace-pre-wrap break-words text-sm text-muted">
              {details.data.rootCause ?? 'No root cause was recorded.'}
            </p>
          </div>

          <div>
            <h4 className="mb-2 text-sm font-semibold text-ink">Recommendations</h4>
            <RecommendationList value={details.data.recommendations} />
          </div>
        </div>
      )}
    </Dialog>
  );
}

function InvestigationRow({
  investigation,
  onOpen,
}: {
  investigation: PreviousInvestigationDto;
  onOpen: () => void;
}) {
  return (
    <button
      type="button"
      onClick={onOpen}
      className="group flex w-full items-start gap-3 rounded-xl border border-line bg-surface-2 px-3 py-3 text-left transition-colors hover:border-primary/40 hover:bg-primary-soft/40"
    >
      <span className="mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary-soft text-primary-glow">
        <Sparkles size={15} />
      </span>
      <span className="min-w-0 flex-1">
        <span className="flex flex-wrap items-center gap-2">
          <span className="truncate text-sm font-medium text-ink">
            {investigation.title}
          </span>
          <InvestigationStatusBadge status={investigation.status} />
        </span>
        {investigation.rootCause && (
          <span className="mt-1 line-clamp-2 block text-xs text-muted">
            {investigation.rootCause}
          </span>
        )}
        <span className="mt-1 block text-[11px] text-muted">
          {formatRelative(investigation.startedAt)}
        </span>
      </span>
      <ChevronRight
        size={16}
        className="mt-1 shrink-0 text-muted transition-transform group-hover:translate-x-0.5 group-hover:text-primary-glow"
      />
    </button>
  );
}

export function InvestigationCard({ refreshKey }: { refreshKey: string }) {
  const investigations = useAsyncData(
    () => applicationService.getInvestigations(APPLICATION_ID, { page: 1, pageSize: 5 }),
    [refreshKey],
  );
  const [selectedId, setSelectedId] = useState<string | null>(null);

  return (
    <>
      <DashboardCard
        title="AI Investigations"
        subtitle="Click an investigation to see the full analysis"
        icon={BrainCircuit}
        action={
          investigations.data && (
            <span className="rounded-full border border-line px-2 py-1 text-[11px] text-muted">
              {investigations.data.totalCount} total
            </span>
          )
        }
      >
        {investigations.loading && <LoadingState label="Loading investigations…" />}
        {!investigations.loading && investigations.error && (
          <ErrorState message={investigations.error} onRetry={investigations.reload} />
        )}
        {!investigations.loading &&
          !investigations.error &&
          investigations.data &&
          investigations.data.investigations.length === 0 && (
            <EmptyState
              icon={<BrainCircuit size={20} />}
              title="No investigations yet"
              description="The agent will create an investigation when it analyses your logs."
            />
          )}
        {!investigations.loading &&
          !investigations.error &&
          investigations.data &&
          investigations.data.investigations.length > 0 && (
            <ul className="flex flex-col gap-2">
              {investigations.data.investigations.map((investigation) => (
                <li key={investigation.id}>
                  <InvestigationRow
                    investigation={investigation}
                    onOpen={() => setSelectedId(investigation.id)}
                  />
                </li>
              ))}
            </ul>
          )}
      </DashboardCard>

      {selectedId && (
        <InvestigationDialog
          investigationId={selectedId}
          onClose={() => setSelectedId(null)}
        />
      )}
    </>
  );
}
