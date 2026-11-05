import type { NotificationStatus } from '@/types/enums';

export interface NotificationDto {
  id: string;
  applicationId: string;
  chatSignature: string;
  text: string;
  notificationStatus: NotificationStatus;
  agentType: string;
  data?: string | null;
  updatedAt: string;
}
