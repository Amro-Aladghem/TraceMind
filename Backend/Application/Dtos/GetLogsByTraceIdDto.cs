namespace Application.Dtos
{
    public class GetLogsByTraceIdRequestDto
    {
        public Guid ApplicationId { get; set; }
        public string TraceId { get; set; } = null!;
    }

    public class GetLogsByTraceIdResponseDto
    {
        public string TraceId { get; set; } = null!;
        public List<LogDto> Logs { get; set; } = [];
    }
}
