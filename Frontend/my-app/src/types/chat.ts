export interface AgentRequestDto {
  chatSignture?: string | null;
  applicationId: string;
  message: string;
  imageUri?: string | null;
  agentType?: string | null;
}

export interface ChatDto {
  chatSignature: string;
  applicationId: string;
  name?: string | null;
  creationTime: string;
  imageUrl?: string | null;
  imageType?: string | null;
  chatRoleId: number;
  message: string;
  data?: string | null;
  agentType?: string | null;
  showMessage: boolean;
  isFinished: boolean;
}
