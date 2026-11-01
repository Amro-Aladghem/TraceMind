using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetApplicationInfoTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetApplicationInfoTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetApplicationInfoTool";
        public Type ArgType => typeof(GetApplicationInfoToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetApplicationInfoToolArg getApplicationInfoToolArg = (GetApplicationInfoToolArg)arg;

            return await ExecuteTool<GetApplicationInfoResponseDto>(async () =>
            {
                return await _logService.GetApplicationInfoAsync(new GetApplicationInfoRequestDto
                {
                    ApplicationId = getApplicationInfoToolArg.applicationId
                }) ?? throw new InvalidOperationException("Application was not found");
            }, Name);
        }
    }

    public class GetApplicationInfoToolArg()
    {
        public Guid applicationId { get; set; }
    }
}
