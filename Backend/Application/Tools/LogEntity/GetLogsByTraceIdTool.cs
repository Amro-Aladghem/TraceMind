using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetLogsByTraceIdTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetLogsByTraceIdTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetLogsByTraceIdTool";
        public Type ArgType => typeof(GetLogsByTraceIdToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetLogsByTraceIdToolArg getLogsByTraceIdToolArg = (GetLogsByTraceIdToolArg)arg;

            return await ExecuteTool<GetLogsByTraceIdResponseDto>(() =>
                _logService.GetLogsByTraceIdAsync(new GetLogsByTraceIdRequestDto
                {
                    ApplicationId = getLogsByTraceIdToolArg.applicationId,
                    TraceId = getLogsByTraceIdToolArg.traceId
                }), Name);
        }
    }

    public class GetLogsByTraceIdToolArg()
    {
        public Guid applicationId { get; set; }
        public string traceId { get; set; } = null!;
    }
}
