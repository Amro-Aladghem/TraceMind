namespace Application.Dtos
{
    public class GetRequestStatisticsRequestDto
    {
        public Guid ApplicationId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class GetRequestStatisticsResponseDto
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageDurationMs { get; set; }
        public double MaxDurationMs { get; set; }

        /// <summary>
        /// Failed requests as a percentage of total requests (0 - 100).
        /// </summary>
        public double ErrorRate { get; set; }
    }
}
