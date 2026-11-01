using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetRecentErrorsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetRecentErrorsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetRecentErrorsTool";
        public Type ArgType => typeof(GetRecentErrorsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetRecentErrorsToolArg getRecentErrorsToolArg = (GetRecentErrorsToolArg)arg;

            return await ExecuteTool<GetRecentErrorsResponseDto>(() =>
                _logService.GetRecentErrorsAsync(new GetRecentErrorsRequestDto
                {
                    ApplicationId = getRecentErrorsToolArg.applicationId,
                    From = getRecentErrorsToolArg.from,
                    Limit = getRecentErrorsToolArg.limit
                }), Name);
        }
    }

    public class GetRecentErrorsToolArg()
    {
        public Guid applicationId { get; set; }
        public DateTime? from { get; set; }
        public int limit { get; set; } = 20;
    }
}
