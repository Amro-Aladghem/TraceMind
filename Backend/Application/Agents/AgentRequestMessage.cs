using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Agents
{
    public class AgentRequestMessage
    {
        public string Text { get; set; } = null!;
        public string Role { get; set; } = null!;    
    }
}
