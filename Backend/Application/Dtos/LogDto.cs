using Domain.Enums;
using System.Text.Json;

namespace Application.Dtos
{
    public class LogDto
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public eLogLevel Level { get; set; }
        public string Message { get; set; } = null!;
        public string? Exception { get; set; }
        public JsonElement? Properties { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public int? StatusCode { get; set; }
        public string? TraceId { get; set; }
        public string? IpAddress { get; set; }
        public string? Domain { get; set; }
    }
}
