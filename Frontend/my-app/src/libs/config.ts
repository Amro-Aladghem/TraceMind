export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5182/api/v1';

export const APPLICATION_ID =
  import.meta.env.VITE_APPLICATION_ID ?? '0f449779-0ec6-44ff-a565-29d3e9008f31';

export const DEFAULT_AGENT_MESSAGE = 'Follow your instructions';

/** The automatic agent call is skipped if it ran less than 10 minutes ago. */
export const DEFAULT_CALL_TTL_MS = 10 * 60 * 1000;

export const NOTIFICATIONS_POLL_INTERVAL_MS = 5000;

export const LOGS_PAGE_SIZE = 20;
