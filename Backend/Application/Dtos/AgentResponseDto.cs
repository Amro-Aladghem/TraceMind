using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.Dtos
{
    public record AgentResponseDto
    {
        public bool IsFinished { get; set; }
        public string Message { get; set; }
        public string? ToolName { get; set; }
        public JsonElement? Data { get; set; }
        public bool AskUser { get; set; }
        public bool HasError { get; set; }
    }
}
