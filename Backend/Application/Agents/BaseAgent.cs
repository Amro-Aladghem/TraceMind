using Application.Dtos;
using Application.Services;
using Application.Services.Coordinators;
using Application.Tools.Coordinator;
using Domain.Enums;
using Infrastructure.AI;
using Infrastructure.AI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;


namespace Application.Agents
{
    public abstract class BaseAgent : IAgent
    {
        private static readonly JsonSerializerOptions DataJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly AgentChatCoordinator _agentChatCoordinator;
        private readonly ToolChatCoordinator _toolChatCoordinator;
        private readonly NotificationService _notificationService;
        private readonly IAgentChat _agentChat;

        protected abstract eAgentType _AgentType { get; }

        private int _aiRetryCount;
        public BaseAgent(
            AgentChatCoordinator agentChatCoordinator, 
            ToolChatCoordinator toolChatCoordinator,
            IConfiguration configuration,
            IAgentChat agentChat,
            NotificationService notificationService
         )
        {
            _agentChatCoordinator = agentChatCoordinator;
            _toolChatCoordinator = toolChatCoordinator;
            _agentChat = agentChat;
            _aiRetryCount = configuration.GetValue<int>("aiRetryCount");
            _notificationService = notificationService; 
        }
       
        public async Task<AgentRunResultDto> RunAsync(AgentRequestDto agentRequestDto)
        {
            int retries = 0;
            AgentResponseDto? agentResponse = new AgentResponseDto();
            AgentRunResultDto agentRunResultDto = new AgentRunResultDto();

            while (!agentRunResultDto.IsFinished)
            {
                agentResponse = await _agentChatCoordinator.GenerateAgentResponse
                (
                    agentRequestDto.ChatSignture!.Value,
                    agentRequestDto.StoreId,
                    _agentChat
                );

                if (agentResponse is null)
                {
                    if (retries < _aiRetryCount)
                    {
                        retries++;
                        continue;
                    }

                    agentRunResultDto.HasError = true;
                    return agentRunResultDto;
                }

                retries = 0;

                agentRunResultDto = await HandleAgentResponse
                (
                    agentResponse,
                    agentRequestDto.StoreId,
                    agentRequestDto.ChatSignture.Value
                );

                if (agentRunResultDto.HasError || agentRunResultDto.AskUser)
                    return agentRunResultDto;
            }

            return agentRunResultDto;
        }

        private async Task<AgentRunResultDto> HandleAgentResponse(AgentResponseDto agentResponseDto, Guid applicationId, Guid chatSignture)
        {
            AgentRunResultDto agentRunResultDto = new AgentRunResultDto();

            if (agentResponseDto.HasError)
                return await GetResultWhenHasError(
                    agentResponseDto,
                    applicationId,
                    chatSignture
                );

            if (agentResponseDto.IsFinished)
                return await GetResultWhenIsFinished(
                     agentResponseDto,
                     applicationId,
                     chatSignture
                );

            if (agentResponseDto.AskUser)
                return await GetResultWhenAskUser(
                    agentResponseDto,
                    applicationId,
                    chatSignture
                );

            if (!string.IsNullOrEmpty(agentResponseDto.ToolName))
            {
                await _toolChatCoordinator.HandleToolExecuteAndSave
                (
                    agentResponseDto.ToolName,
                    agentResponseDto.Data ?? new JsonElement(),
                    chatSignture:chatSignture,
                    applicationId:applicationId,
                    agentType:_AgentType
                );
            }

            return agentRunResultDto;
        }

        private async Task<AgentRunResultDto> GetResultWhenHasError(AgentResponseDto agentResponseDto,Guid applicationId, Guid chatSignture)
        {
            AgentRunResultDto agentRunResultDto = new AgentRunResultDto();
            agentRunResultDto.HasError = true;

            await _notificationService.CreateNotification(applicationId, new NotificationDto()
            {
                ChatSignature = chatSignture,
                AgentType = _AgentType.ToString(),
                ApplicationId = applicationId,
                NotificationStatus = eNotificationStatus.Failed.ToString(),
                Text = agentResponseDto.Message,
            });

            return agentRunResultDto;
        }

        private async Task<AgentRunResultDto> GetResultWhenIsFinished(AgentResponseDto agentResponseDto, Guid applicationId, Guid chatSignture)
        {
            AgentRunResultDto agentRunResultDto = new AgentRunResultDto();
            agentRunResultDto.IsFinished = true;
            agentRunResultDto.Message = agentResponseDto.Message;

            await _notificationService.CreateNotification(applicationId, new NotificationDto()
            {
                ChatSignature = chatSignture,
                AgentType = _AgentType.ToString(),
                ApplicationId = applicationId,
                NotificationStatus = eNotificationStatus.Completed.ToString(),
                Data= null,
                Text = agentResponseDto.Message,
            });

            return agentRunResultDto;
        }

        private async Task<AgentRunResultDto> GetResultWhenAskUser(AgentResponseDto agentResponseDto, Guid applicationId, Guid chatSignture)
        {
            AgentRunResultDto agentRunResultDto = new AgentRunResultDto();
            agentRunResultDto.AskUser = true;
            agentRunResultDto.Message = agentResponseDto.Message;

            await _notificationService.CreateNotification(applicationId, new NotificationDto()
            {
                ChatSignature = chatSignture,
                AgentType = _AgentType.ToString(),
                ApplicationId = applicationId,
                NotificationStatus = eNotificationStatus.WaitingForUser.ToString(),
                Text = agentResponseDto.Message,
                Data = null
            });

            return agentRunResultDto;
        }
    }
}
