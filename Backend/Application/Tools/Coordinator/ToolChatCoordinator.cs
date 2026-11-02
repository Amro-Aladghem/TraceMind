using Application.Dtos;
using Application.Services;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.Tools.Coordinator
{
    public class ToolChatCoordinator
    {
        private readonly ToolExecutor _toolExecutor;
        private readonly ChatService _chatService;

        public ToolChatCoordinator(ToolExecutor toolExecutor, ChatService chatService)
        {
            _toolExecutor = toolExecutor;
            _chatService = chatService;
        }


        public async Task<ToolResponse> HandleToolExecuteAndSave(string toolName, JsonElement data, Guid chatSignture, Guid applicationId, eAgentType agentType)
        {
            ToolResponse toolResponse = await _toolExecutor.HandleExecuteTool(toolName, data);

            ChatDto chatDto = await _chatService.CreateChat(applicationId, new ChatDto()
            {
                Message = toolResponse.Message,
                ChatRoleId = (int)eChatRole.Tool,
                AgentType = agentType.ToString(),
                ChatSignature = chatSignture,
                ApplicationId = applicationId,
                CreationTime = DateTime.UtcNow,
                Data = $"The Result Data From Tool With Name:${toolName}:" + toolResponse.Data
            });

            return toolResponse;
        }

    }
}
