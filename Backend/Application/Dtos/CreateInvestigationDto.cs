using Domain.Enums;
using System.Text.Json;

namespace Application.Dtos
{
    public class CreateInvestigationRequestDto
    {
        public Guid ApplicationId { get; set; }
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string? RootCause { get; set; }
        public JsonElement? Recommendations { get; set; }
        public double? Confidence { get; set; }
        public string? ModelName { get; set; }
    }

    public class CreateInvestigationResponseDto
    {
        public Guid Id { get; set; }
        public eInvestigationStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
