namespace Application.Dtos
{
    public class CreateApplicationRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Domain { get; set; }
        public string? Environment { get; set; }
        public string? IpAddress { get; set; }
    }
}
