using Domain.Enums;

namespace Application.Dtos
{
    public class GetPreviousInvestigationsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public eInvestigationStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetPreviousInvestigationsResponseDto
    {
        public List<PreviousInvestigationDto> Investigations { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public class PreviousInvestigationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string? RootCause { get; set; }
        public eInvestigationStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
