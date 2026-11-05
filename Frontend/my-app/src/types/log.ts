import type { LogLevel } from '@/types/enums';

export interface LogDto {
  id: string;
  timestamp: string;
  level: LogLevel;
  message: string;
  exception?: string | null;
  properties?: unknown;
  requestPath?: string | null;
  requestMethod?: string | null;
  statusCode?: number | null;
  traceId?: string | null;
  ipAddress?: string | null;
  domain?: string | null;
}

export interface GetLogsResponseDto {
  logs: LogDto[];
  totalCount: number;
}

export interface GetLogsParams {
  level?: LogLevel;
  from?: string;
  to?: string;
  requestPath?: string;
  requestMethod?: string;
  statusCode?: number;
  traceId?: string;
  page?: number;
  pageSize?: number;
}
