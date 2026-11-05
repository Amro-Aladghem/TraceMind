import api from '@/services/api';
import type { ApiResponse } from '@/types/api';
import type { AgentRequestDto } from '@/types/chat';
import type {
  GetLogsParams,
  GetLogsResponseDto,
} from '@/types/log';
import type {
  GetPreviousInvestigationsParams,
  GetPreviousInvestigationsResponseDto,
  InvestigationDetailsDto,
} from '@/types/investigation';
import type { NotificationDto } from '@/types/notification';

const baseUri = '/applications';

const toFailure = (error: unknown): ApiResponse<never> => {
  const { message, statusCode } = error as { message: string; statusCode: number };
  return { isSuccess: false, error: message, statusCode };
};

const cleanParams = (params: Record<string, unknown>): Record<string, unknown> =>
  Object.fromEntries(
    Object.entries(params).filter(([, value]) => value !== undefined && value !== ''),
  );

export const applicationService = {
  async chatWithAgent(
    applicationId: string,
    chatSignature: string,
    agentRequestDto: AgentRequestDto,
  ): Promise<ApiResponse<string>> {
    try {
      const response = await api.post<string>(
        `${baseUri}/${applicationId}/chats/${chatSignature}`,
        agentRequestDto,
      );
      return { isSuccess: true, data: response.data };
    } catch (error) {
      return toFailure(error);
    }
  },

  async getLogs(
    applicationId: string,
    params: GetLogsParams,
  ): Promise<ApiResponse<GetLogsResponseDto>> {
    try {
      const response = await api.get<GetLogsResponseDto>(
        `${baseUri}/${applicationId}/logs`,
        { params: cleanParams({ ...params }) },
      );
      return { isSuccess: true, data: response.data };
    } catch (error) {
      return toFailure(error);
    }
  },

  async getInvestigations(
    applicationId: string,
    params: GetPreviousInvestigationsParams = {},
  ): Promise<ApiResponse<GetPreviousInvestigationsResponseDto>> {
    try {
      const response = await api.get<GetPreviousInvestigationsResponseDto>(
        `${baseUri}/${applicationId}/investigations`,
        { params: cleanParams({ ...params }) },
      );
      return { isSuccess: true, data: response.data };
    } catch (error) {
      return toFailure(error);
    }
  },

  async getInvestigationById(
    applicationId: string,
    investigationId: string,
  ): Promise<ApiResponse<InvestigationDetailsDto>> {
    try {
      const response = await api.get<InvestigationDetailsDto>(
        `${baseUri}/${applicationId}/investigations/${investigationId}`,
      );
      return { isSuccess: true, data: response.data };
    } catch (error) {
      return toFailure(error);
    }
  },

  async getNotifications(applicationId: string): Promise<ApiResponse<NotificationDto[]>> {
    try {
      const response = await api.get<NotificationDto[]>(
        `${baseUri}/${applicationId}/notifications`,
      );
      return { isSuccess: true, data: Array.isArray(response.data) ? response.data : [] };
    } catch (error) {
      return toFailure(error);
    }
  },
};

export default applicationService;
