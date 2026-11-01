using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Domain.Enities
{
    public class AIInvestigation
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string? RootCause { get; set; }
        public JsonElement? Recommendations { get; set; }
        public double? Confidence { get; set; }
        public eInvestigationStatus Status { get; set; }
        public string? ModelName { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Application Application { get; set; } = null!;
    }
}
