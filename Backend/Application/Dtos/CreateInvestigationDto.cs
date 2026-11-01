using Domain.Enums;

namespace Application.Dtos
{
    public class CreateInvestigationRequestDto
    {
        public Guid ApplicationId { get; set; }
        public string Title { get; set; } = null!;
    }

    public class CreateInvestigationResponseDto
    {
        public Guid Id { get; set; }
        public eInvestigationStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
