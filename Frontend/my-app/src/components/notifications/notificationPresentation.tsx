import type { LucideIcon } from 'lucide-react';
import {
  AlertTriangle,
  CheckCircle2,
  Clock,
  Loader2,
  MessageCircle,
  Radar,
} from 'lucide-react';
import type { NotificationStatus } from '@/types/enums';

interface AgentPresentation {
  Icon: LucideIcon;
  tint: string;
  label: string;
}

export const agentPresentation: Record<string, AgentPresentation> = {
  LoggingAgent: {
    Icon: Radar,
    tint: 'bg-primary-soft text-primary-glow',
    label: 'Logging Agent',
  },
};

interface StatusPresentation {
  Icon: LucideIcon;
  tint: string;
  spin?: boolean;
}

export const statusPresentation: Record<NotificationStatus, StatusPresentation> = {
  Queued: { Icon: Clock, tint: 'text-muted' },
  Running: { Icon: Loader2, tint: 'text-primary-glow', spin: true },
  WaitingForUser: { Icon: MessageCircle, tint: 'text-warning' },
  Completed: { Icon: CheckCircle2, tint: 'text-success' },
  Failed: { Icon: AlertTriangle, tint: 'text-danger' },
};

export const statusLabel: Record<NotificationStatus, string> = {
  Queued: 'Queued',
  Running: 'Running',
  WaitingForUser: 'Waiting for you',
  Completed: 'Completed',
  Failed: 'Failed',
};

export function getAgentPresentation(agentType: string): AgentPresentation {
  return (
    agentPresentation[agentType] ?? {
      ...agentPresentation.LoggingAgent,
      label: agentType,
    }
  );
}
