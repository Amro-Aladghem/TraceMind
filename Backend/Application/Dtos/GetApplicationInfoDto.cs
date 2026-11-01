namespace Application.Dtos
{
    public class GetApplicationInfoRequestDto
    {
        public Guid ApplicationId { get; set; }
    }

    public class GetApplicationInfoResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Domain { get; set; }
        public string? Environment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
