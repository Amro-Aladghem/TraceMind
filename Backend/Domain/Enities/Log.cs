using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Domain.Enities
{
    public class Log
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
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

        /// <summary>
        /// Request duration in milliseconds, populated by Serilog request logging.
        /// </summary>
        public double? DurationMs { get; set; }

        public Application Application { get; set; } = null!;
    }
}
