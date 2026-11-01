using Domain.Enums;

namespace Application.Dtos
{
    public class SearchLogsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public string SearchTerm { get; set; } = null!;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public eLogLevel? Level { get; set; }
        public int Limit { get; set; } = 50;
    }

    public class SearchLogsResponseDto
    {
        public List<LogDto> Logs { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
