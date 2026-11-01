namespace Application.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid ChatSignature { get; set; }
        public string Text { get; set; } = string.Empty;
        public string NotificationStatus { get; set; } = "Queued";
        public string AgentType { get; set; } = null!;
        public string? Data { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
