using Infrastructure.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class AgentRequestDto
    {
        public Guid? ChatSignture { get; set; }
        public Guid ApplicationId { get; set; }
        public string Message { get; set; } = null!;
        public string?ImageUri { get; set; }
        public string?AgentType { get; set;  }
    }
}
