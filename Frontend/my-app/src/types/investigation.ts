import type { InvestigationStatus } from '@/types/enums';

export interface PreviousInvestigationDto {
  id: string;
  title: string;
  summary?: string | null;
  rootCause?: string | null;
  status: InvestigationStatus;
  startedAt: string;
  completedAt?: string | null;
}

export interface GetPreviousInvestigationsResponseDto {
  investigations: PreviousInvestigationDto[];
  totalCount: number;
}

export interface GetPreviousInvestigationsParams {
  from?: string;
  to?: string;
  status?: InvestigationStatus;
  page?: number;
  pageSize?: number;
}

export interface InvestigationDetailsDto {
  id: string;
  applicationId: string;
  title: string;
  summary?: string | null;
  rootCause?: string | null;
  recommendations?: unknown;
  confidence?: number | null;
  status: InvestigationStatus;
  modelName?: string | null;
  startedAt: string;
  completedAt?: string | null;
}
