using Domain.Enities;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid ChatSignature { get; set; }
        public string? Name { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid ApplicationId { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageType { get; set; }
        public int ChatRoleId { get; set; }
        public string Message { get; set; } = null!;
        public string? Data { get; set; }
        public eAgentType? AgentType { get; set; }
        public bool ShowMessage { get; set; } = false;
        public bool IsFinished { get; set; } = false;

        public ChatRole ChatRole { get; set; } = null!;
        public Application Application { get; set; } = null!;
    }
}
