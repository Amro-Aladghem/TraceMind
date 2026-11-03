using Application.Dtos;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace Presentation
{
    [Route("api/v1/applications")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ChatService _chatService;
        private readonly NotificationService _notificationService;
        private readonly LogService _logService;
        private readonly AIInvestigationService _investigationService;
        private readonly Channel<AgentRequestDto> _channel;

        public ApplicationController(
            ChatService chatService,
            NotificationService notificationService,
            LogService logService,
            AIInvestigationService investigationService,
            Channel<AgentRequestDto> channel)
        {
            _chatService = chatService;
            _notificationService = notificationService;
            _logService = logService;
            _investigationService = investigationService;
            _channel = channel;
        }

        [HttpPost("{applicationId}/chats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatDto>> CreateChat(Guid applicationId, ChatDto chatDto)
        {
            chatDto.ApplicationId = applicationId;

            var result = await _chatService.CreateChat(applicationId, chatDto);

            return Ok(result);
        }

        [HttpPost("{applicationId}/chats/{chatSignature}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ChatWithAgent(AgentRequestDto agentRequestDto, Guid applicationId, Guid chatSignature)
        {
            string agentType = eAgentType.LoggingAgent.ToString();

            await _chatService.CreateChat(applicationId, new ChatDto()
            {
                ChatSignature = chatSignature,
                ApplicationId = applicationId,
                ChatRoleId = (int)eChatRole.User,
                Message = agentRequestDto.Message,
                ImageUrl = agentRequestDto.ImageUri,
                AgentType = agentType,
                ShowMessage = true,
                CreationTime = DateTime.UtcNow
            });

            await _notificationService.CreateNotification(applicationId, new NotificationDto()
            {
                ApplicationId = applicationId,
                ChatSignature = chatSignature,
                AgentType = agentType,
                NotificationStatus = eNotificationStatus.Queued.ToString(),
                Text = "Message queued for LoggingAgent"
            });

            agentRequestDto.AgentType = agentType;
            agentRequestDto.ApplicationId = applicationId;
            agentRequestDto.ChatSignture = chatSignature;

            await _channel.Writer.WriteAsync(agentRequestDto);

            return Accepted(agentType);
        }

        [HttpGet("{applicationId}/chats/{chatSignature}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ChatDto>>> GetChatHistory(Guid applicationId, Guid chatSignature)
        {
            var result = await _chatService.GetChatsForChatId(applicationId, chatSignature);

            return Ok(result);
        }

        [HttpGet("{applicationId}/logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetLogsResponseDto>> GetLogs(Guid applicationId, [FromQuery] GetLogsRequestDto request)
        {
            request.ApplicationId = applicationId;

            var result = await _logService.GetLogsAsync(request);

            return Ok(result);
        }

        [HttpGet("{applicationId}/investigations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPreviousInvestigationsResponseDto>> GetInvestigations(
            Guid applicationId,
            [FromQuery] GetPreviousInvestigationsRequestDto request)
        {
            request.ApplicationId = applicationId;

            var result = await _investigationService.GetPreviousInvestigationsAsync(request);

            return Ok(result);
        }

        [HttpGet("{applicationId}/investigations/{investigationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetInvestigationByIdResponseDto>> GetInvestigation(
            Guid applicationId,
            Guid investigationId)
        {
            var result = await _investigationService.GetInvestigationByIdAsync(new GetInvestigationByIdRequestDto()
            {
                InvestigationId = investigationId
            });

            if (result is null || result.ApplicationId != applicationId)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{applicationId}/notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
            Guid applicationId,
            [FromQuery] Guid? chatSignature = null)
        {
            if (chatSignature.HasValue)
            {
                var chatNotifications = await _notificationService.GetNotificationsForChatId(applicationId, chatSignature.Value);

                return Ok(chatNotifications);
            }

            var notifications = await _notificationService.GetNotificationsForApplicationId(applicationId);

            return Ok(notifications);
        }
    }
}
