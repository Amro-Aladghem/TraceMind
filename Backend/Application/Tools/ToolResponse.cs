namespace Application.Tools
{
    public record ToolResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Data { get; set; }
    }
}
