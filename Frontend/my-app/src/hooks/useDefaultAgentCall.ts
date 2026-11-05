import { useEffect, useRef } from 'react';
import { useNotifications } from '@/contexts/NotificationsContext';
import { APPLICATION_ID, DEFAULT_AGENT_MESSAGE } from '@/libs/config';
import {
  clearDefaultAgentCall,
  getOrCreateChatSignature,
  markDefaultAgentCall,
  shouldRunDefaultAgentCall,
} from '@/libs/storage';
import applicationService from '@/services/applicationService';

/**
 * On the first dashboard visit (and at most once every 10 minutes), ask the
 * LoggingAgent to follow its instructions. The chat signature lives in the
 * browser session and is created on demand.
 *
 * If the request fails, the timestamp is cleared so the next visit can retry.
 */
export function useDefaultAgentCall(): void {
  const { reload } = useNotifications();
  const startedRef = useRef(false);

  useEffect(() => {
    if (startedRef.current) return;
    if (!shouldRunDefaultAgentCall()) return;

    startedRef.current = true;
    markDefaultAgentCall();

    const chatSignature = getOrCreateChatSignature();

    void applicationService
      .chatWithAgent(APPLICATION_ID, chatSignature, {
        applicationId: APPLICATION_ID,
        chatSignture: chatSignature,
        message: DEFAULT_AGENT_MESSAGE,
        imageUri: null,
        agentType: 'LoggingAgent',
      })
      .then((result) => {
        if (result.isSuccess) {
          reload();
          return;
        }
        clearDefaultAgentCall();
      })
      .catch(() => {
        clearDefaultAgentCall();
      });
  }, [reload]);
}
