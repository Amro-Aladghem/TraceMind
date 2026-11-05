export type LogLevel =
  | 'Verbose'
  | 'Debug'
  | 'Information'
  | 'Warning'
  | 'Error'
  | 'Fatal';

export const logLevels: LogLevel[] = [
  'Verbose',
  'Debug',
  'Information',
  'Warning',
  'Error',
  'Fatal',
];

export type InvestigationStatus = 'Pending' | 'Running' | 'Completed' | 'Failed';

export const investigationStatuses: InvestigationStatus[] = [
  'Pending',
  'Running',
  'Completed',
  'Failed',
];

export type NotificationStatus =
  | 'Queued'
  | 'Running'
  | 'WaitingForUser'
  | 'Completed'
  | 'Failed';

export type AgentType = 'LoggingAgent';

/**
 * Notifications in these states mean the agent is no longer working.
 * Mirrors the DealerAgent frontend behaviour.
 */
export const terminalNotificationStatuses: NotificationStatus[] = [
  'WaitingForUser',
  'Completed',
  'Failed',
];
