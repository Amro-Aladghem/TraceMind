using Domain.Enities;
using Domain.Enums;
using System;

namespace Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid ChatSignture { get; set; }

        public string Text { get; set; } = string.Empty;

        public eNotificationStatus NotficationStatus { get; set; }
        public eAgentType AgentType { get; set; }
        public string? Data { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Application Application { get; set; } = null!;
    }
}
