using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetSlowRequestsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetSlowRequestsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetSlowRequestsTool";
        public Type ArgType => typeof(GetSlowRequestsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetSlowRequestsToolArg getSlowRequestsToolArg = (GetSlowRequestsToolArg)arg;

            return await ExecuteTool<GetSlowRequestsResponseDto>(() =>
                _logService.GetSlowRequestsAsync(new GetSlowRequestsRequestDto
                {
                    ApplicationId = getSlowRequestsToolArg.applicationId,
                    From = getSlowRequestsToolArg.from,
                    To = getSlowRequestsToolArg.to,
                    MinDurationMs = getSlowRequestsToolArg.minDurationMs,
                    Limit = getSlowRequestsToolArg.limit
                }), Name);
        }
    }

    public class GetSlowRequestsToolArg()
    {
        public Guid applicationId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public double minDurationMs { get; set; } = 2000;
        public int limit { get; set; } = 20;
    }
}
