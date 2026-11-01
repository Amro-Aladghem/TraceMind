namespace Application.Dtos
{
    public class GetRecentErrorsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public DateTime? From { get; set; }
        public int Limit { get; set; } = 20;
    }

    public class GetRecentErrorsResponseDto
    {
        public List<LogDto> Errors { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
