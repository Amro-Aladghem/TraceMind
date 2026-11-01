namespace Application.Dtos
{
    public class GetErrorCountRequestDto
    {
        public Guid ApplicationId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class GetErrorCountResponseDto
    {
        public int TotalErrors { get; set; }
        public int ErrorCount { get; set; }
        public int FatalCount { get; set; }
    }
}
