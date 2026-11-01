using Domain.Enums;

namespace Application.Dtos
{
    public class UpdateInvestigationStatusRequestDto
    {
        public Guid InvestigationId { get; set; }
        public eInvestigationStatus Status { get; set; }
    }

    public class UpdateInvestigationStatusResponseDto
    {
        public Guid Id { get; set; }
        public eInvestigationStatus Status { get; set; }
        public bool Success { get; set; }
    }
}
