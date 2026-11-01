using Domain.Enums;
using System.Text.Json;

namespace Application.Dtos
{
    public class GetInvestigationByIdRequestDto
    {
        public Guid InvestigationId { get; set; }
    }

    public class GetInvestigationByIdResponseDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string? RootCause { get; set; }
        public JsonElement? Recommendations { get; set; }
        public eInvestigationStatus Status { get; set; }
        public string? ModelName { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
