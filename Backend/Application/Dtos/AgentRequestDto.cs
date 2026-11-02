using Infrastructure.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class AgentRequestDto
    {
        public Guid? ChatSignture { get; set; }
        public Guid StoreId { get; set; }
        public string Message { get; set; }
        public string?ImageUri { get; set; }
        public string?AgentType { get; set;  }
    }
}
