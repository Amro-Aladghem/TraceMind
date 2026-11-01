namespace Application.Dtos
{
    public class GetSlowRequestsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double MinDurationMs { get; set; } = 2000;
        public int Limit { get; set; } = 20;
    }

    public class SlowRequestDto
    {
        public string RequestPath { get; set; } = null!;
        public string RequestMethod { get; set; } = null!;
        public int? StatusCode { get; set; }
        public double DurationMs { get; set; }
        public DateTime Timestamp { get; set; }
        public string? TraceId { get; set; }
    }

    public class GetSlowRequestsResponseDto
    {
        public List<SlowRequestDto> Requests { get; set; } = [];
    }
}
