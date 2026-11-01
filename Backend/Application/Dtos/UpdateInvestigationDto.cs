using System.Text.Json;

namespace Application.Dtos
{
    public class UpdateInvestigationRequestDto
    {
        public Guid InvestigationId { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string RootCause { get; set; } = null!;
        public JsonElement? Recommendations { get; set; }
        public string? ModelName { get; set; }
    }

    public class UpdateInvestigationResponseDto
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
    }
}
