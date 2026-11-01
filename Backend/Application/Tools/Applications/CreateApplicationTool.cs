using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Applications
{
    public class CreateApplicationTool : BaseTool, IAgentTool
    {
        private readonly ApplicationService _applicationService;

        public CreateApplicationTool(ApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public string Name => "CreateApplicationTool";
        public Type ArgType => typeof(CreateApplicationToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            CreateApplicationToolArg createApplicationToolArg = (CreateApplicationToolArg)arg;

            return await ExecuteTool<GetApplicationInfoResponseDto>(() =>
                _applicationService.CreateApplicationAsync(new CreateApplicationRequestDto
                {
                    Name = createApplicationToolArg.name,
                    Domain = createApplicationToolArg.domain,
                    Environment = createApplicationToolArg.environment,
                    IpAddress = createApplicationToolArg.ipAddress
                }), Name);
        }
    }

    public class CreateApplicationToolArg()
    {
        public string name { get; set; } = null!;
        public string? domain { get; set; }
        public string? environment { get; set; }
        public string? ipAddress { get; set; }
    }
}
