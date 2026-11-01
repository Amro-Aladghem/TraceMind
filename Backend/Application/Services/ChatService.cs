using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ChatService
    {
        private readonly AppDbContext _context;

        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatDto> CreateChat(Guid applicationId, ChatDto dto)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                ChatSignature = dto.ChatSignature,
                ApplicationId = applicationId,
                Name = dto.Name,
                CreationTime = DateTime.UtcNow,
                ImageUrl = dto.ImageUrl,
                ImageType = dto.ImageType,
                ChatRoleId = dto.ChatRoleId,
                Message = dto.Message,
                Data = dto.Data,
                AgentType = string.IsNullOrWhiteSpace(dto.AgentType) ? null : Enum.Parse<eAgentType>(dto.AgentType, true),
                ShowMessage = dto.ShowMessage,
                IsFinished = dto.IsFinished
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return ToDto(chat);
        }

        public async Task<List<ChatDto>?> GetChatsForChatId(Guid applicationId, Guid chatSignature)
        {
            List<ChatDto> chats = await _context.Chats
                .Where(c => c.ApplicationId == applicationId && c.ChatSignature == chatSignature)
                .OrderBy(c => c.CreationTime)
                .Select(c => ToDto(c))
                .ToListAsync();

            if (chats.Count == 0)
                return null;

            return chats;
        }

        private static ChatDto ToDto(Chat c)
        {
            return new ChatDto
            {
                ChatSignature = c.ChatSignature,
                ApplicationId = c.ApplicationId,
                Name = c.Name,
                CreationTime = c.CreationTime,
                ImageUrl = c.ImageUrl,
                ImageType = c.ImageType,
                ChatRoleId = c.ChatRoleId,
                Message = c.Message,
                Data = c.Data,
                AgentType = c.AgentType.HasValue ? c.AgentType.Value.ToString() : null,
                ShowMessage = c.ShowMessage,
                IsFinished = c.IsFinished
            };
        }
    }
}
