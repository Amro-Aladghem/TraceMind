import { APPLICATION_ID, DEFAULT_CALL_TTL_MS } from '@/libs/config';

const CHAT_SIGNATURE_KEY = `tracemind.chatSignature.${APPLICATION_ID}`;
const LAST_CALL_KEY = `tracemind.lastAgentCallAt.${APPLICATION_ID}`;

function safeSessionGet(key: string): string | null {
  try {
    const value = window.sessionStorage.getItem(key);
    return value && value.trim().length > 0 ? value : null;
  } catch {
    return null;
  }
}

function safeSessionSet(key: string, value: string): void {
  try {
    window.sessionStorage.setItem(key, value);
  } catch {
    // ignore storage failures
  }
}

function safeLocalGet(key: string): string | null {
  try {
    return window.localStorage.getItem(key);
  } catch {
    return null;
  }
}

function safeLocalSet(key: string, value: string): void {
  try {
    window.localStorage.setItem(key, value);
  } catch {
    // ignore storage failures
  }
}

/** Returns the chat signature stored in this browser session, creating one if needed. */
export function getOrCreateChatSignature(): string {
  const existing = safeSessionGet(CHAT_SIGNATURE_KEY);
  if (existing) return existing;

  const signature =
    typeof crypto !== 'undefined' && 'randomUUID' in crypto
      ? crypto.randomUUID()
      : `${Date.now()}-${Math.random().toString(16).slice(2)}`;

  safeSessionSet(CHAT_SIGNATURE_KEY, signature);
  return signature;
}

export function getChatSignature(): string | null {
  return safeSessionGet(CHAT_SIGNATURE_KEY);
}

/** True when the automatic agent call is allowed (never ran or TTL expired). */
export function shouldRunDefaultAgentCall(now = Date.now()): boolean {
  const raw = safeLocalGet(LAST_CALL_KEY);
  if (!raw) return true;

  const last = Number(raw);
  if (!Number.isFinite(last)) return true;

  return now - last >= DEFAULT_CALL_TTL_MS;
}

export function markDefaultAgentCall(now = Date.now()): void {
  safeLocalSet(LAST_CALL_KEY, String(now));
}

export function clearDefaultAgentCall(): void {
  try {
    window.localStorage.removeItem(LAST_CALL_KEY);
  } catch {
    // ignore storage failures
  }
}
