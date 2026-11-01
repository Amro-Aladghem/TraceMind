using Domain.Enums;

namespace Application.Dtos
{
    public class GetLogsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public eLogLevel? Level { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? TraceId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetLogsResponseDto
    {
        public List<LogDto> Logs { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
