using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDto> CreateNotification(Guid applicationId, NotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                ChatSignture = dto.ChatSignature,
                Text = dto.Text,
                NotficationStatus = string.IsNullOrWhiteSpace(dto.NotificationStatus)
                    ? eNotificationStatus.Queued
                    : Enum.Parse<eNotificationStatus>(dto.NotificationStatus, true),
                AgentType = Enum.Parse<eAgentType>(dto.AgentType, true),
                Data = dto.Data,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return ToDto(notification);
        }

        public async Task<List<NotificationDto>> GetNotificationsForChatId(Guid applicationId, Guid chatSignature)
        {
            return await _context.Notifications
                .Where(n => n.ApplicationId == applicationId && n.ChatSignture == chatSignature)
                .OrderBy(n => n.UpdatedAt)
                .Select(n => ToDto(n))
                .ToListAsync();
        }

        public async Task<List<NotificationDto>> GetNotificationsForApplicationId(Guid applicationId)
        {
            return await _context.Notifications
                .Where(n => n.ApplicationId == applicationId)
                .OrderBy(n => n.UpdatedAt)
                .Select(n => ToDto(n))
                .ToListAsync();
        }

        public async Task<List<NotificationDto>> GetNotificationsForSpecificStatus(Guid applicationId, eNotificationStatus status)
        {
            return await _context.Notifications
                .Where(n => n.ApplicationId == applicationId && n.NotficationStatus == status)
                .OrderBy(n => n.UpdatedAt)
                .Select(n => ToDto(n))
                .ToListAsync();
        }

        public async Task<NotificationDto?> UpdateNotificationStatus(Guid applicationId, Guid notificationId, eNotificationStatus status)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.ApplicationId == applicationId);

            if (notification is null)
                return null;

            notification.NotficationStatus = status;
            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ToDto(notification);
        }

        private static NotificationDto ToDto(Notification n)
        {
            return new NotificationDto
            {
                Id = n.Id,
                ApplicationId = n.ApplicationId,
                ChatSignature = n.ChatSignture,
                Text = n.Text,
                NotificationStatus = n.NotficationStatus.ToString(),
                AgentType = n.AgentType.ToString(),
                Data = n.Data,
                UpdatedAt = n.UpdatedAt
            };
        }
    }
}
