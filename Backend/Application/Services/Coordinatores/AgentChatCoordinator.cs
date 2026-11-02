using Application.Agents;
using Application.Dtos;
using Domain.Enums;
using Infrastructure.AI;
using Infrastructure.AI.Chat;
using Infrastructure.AI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.Services.Coordinators
{
    public class AgentChatCoordinator
    {
        private static readonly JsonSerializerOptions DataJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly ChatService _chatService;

        public AgentChatCoordinator(ChatService chatService)
        {
            _chatService = chatService;
        }

        private int GetChatRoleIdByIAgentChat(IAgentChat agentChat)
        {
            int roleId = agentChat switch
            {
                LoggingAgentChat=>(int)eAgentType.LoggingAgent,
                _=> throw new Exception("agent chat parameter is not IAgentChat")
            };

            return roleId;
        }

        private eAgentType GetAgentTypeByIAgentChat(IAgentChat agentChat)
        {
            return agentChat switch
            {
                LoggingAgentChat=>eAgentType.LoggingAgent,
                _ => throw new Exception("agent chat parameter is not IAgentChat")
            };
        }

        private List<ChatDto>? FilterChatHistoryByAgentType(List<ChatDto>?chatHistory, eAgentType agentType)
        {
            if (chatHistory is null)
                return null;

            string agentTypeName = agentType.ToString();

            List<ChatDto>? chats = chatHistory.Where(c => c.AgentType == agentTypeName)
                 .OrderBy(c => c.CreationTime)
                 .ToList();

            return chats;
        }

        public async Task<AgentResponseDto?> GenerateAgentResponse(Guid chatSignture,Guid applicationId,IAgentChat agentChat)
        {
            try
            {
                eAgentType agentType = GetAgentTypeByIAgentChat(agentChat);

                List<ChatDto>? chatHistory = await _chatService.GetChatsForChatId(applicationId, chatSignture);

                chatHistory = FilterChatHistoryByAgentType(chatHistory, agentType);

                string? response = await agentChat.Chat(applicationId, chatHistory);

                if (response is null)
                    return null;

                var options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = false
                };

                AgentResponseDto agentResponseDto =  JsonSerializer.Deserialize<AgentResponseDto>(response, options)!;

                ChatDto chatDto = await _chatService.CreateChat(applicationId, new ChatDto()
                {
                    Message = response,
                    ChatRoleId = GetChatRoleIdByIAgentChat(agentChat),
                    AgentType = agentType.ToString(),
                    ChatSignature = chatSignture,
                    ApplicationId = applicationId,
                    CreationTime = DateTime.UtcNow,
                    ShowMessage = agentResponseDto.IsFinished || agentResponseDto.AskUser,
                    Data = null,
                    IsFinished = agentResponseDto.IsFinished,
                });

                return agentResponseDto;
            }
            catch
            {
                return null;
            }
        }
    }
}
