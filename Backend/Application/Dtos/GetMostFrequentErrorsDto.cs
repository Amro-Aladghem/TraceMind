namespace Application.Dtos
{
    public class GetMostFrequentErrorsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Limit { get; set; } = 10;
    }

    public class FrequentErrorDto
    {
        public string ExceptionType { get; set; } = null!;
        public string SampleMessage { get; set; } = null!;
        public int Count { get; set; }
        public DateTime LastOccurredAt { get; set; }
    }

    public class GetMostFrequentErrorsResponseDto
    {
        public List<FrequentErrorDto> Errors { get; set; } = [];
    }
}
