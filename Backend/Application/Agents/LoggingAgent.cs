using Application.Services;
using Application.Services.Coordinators;
using Application.Tools.Coordinator;
using Domain.Enums;
using Infrastructure.AI.Chat;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Agents
{
    public class LoggingAgent :BaseAgent , IAgent
    {
        public LoggingAgent(AgentChatCoordinator agentChatCoordinator, ToolChatCoordinator toolChatCoordinator, IConfiguration configuration
            , LoggingAgentChat loggingAgentChat, NotificationService notificationService)
            : base(agentChatCoordinator, toolChatCoordinator, configuration, loggingAgentChat, notificationService) { }

        protected override eAgentType _AgentType => eAgentType.LoggingAgent;
    }
}
