namespace Application.Dtos
{
    public class DailyErrorLogDto
    {
        public string? RequestPath { get; set; }

        public string Message { get; set; } = null!;

        public long Count { get; set; }
    }
}
