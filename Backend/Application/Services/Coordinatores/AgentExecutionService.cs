using Application.Agents;
using Application.Dtos;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Coordinators
{
    public class AgentExecutionService
    {
        private readonly LoggingAgent _loggingAgent;
        public AgentExecutionService(LoggingAgent loggingAgent)
        {
            _loggingAgent = loggingAgent;
        }

        public async Task<AgentRunResultDto> ExecutAgent(AgentRequestDto agentRequestDto,string agentType)
        {
            eAgentType type = Enum.Parse<eAgentType>(agentType);

            BaseAgent agent = type switch
            {
                eAgentType.LoggingAgent => _loggingAgent,
                _ => throw new ArgumentOutOfRangeException(nameof(agentType))
            };

            return await agent.RunAsync(agentRequestDto);
        }
    }
}
