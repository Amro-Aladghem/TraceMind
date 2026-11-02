using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.Dtos
{
    public class AgentRunResultDto
    {
        public bool IsFinished { get; set; } = false;
        public string Message { get; set; }
        public bool AskUser { get; set; } = false;
        public bool HasError { get; set; } = false;
    }
}
