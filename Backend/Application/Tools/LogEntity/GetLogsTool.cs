using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;
using Domain.Enums;

namespace Application.Tools.Logs
{
    public class GetLogsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetLogsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetLogsTool";
        public Type ArgType => typeof(GetLogsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetLogsToolArg getLogsToolArg = (GetLogsToolArg)arg;

            return await ExecuteTool<GetLogsResponseDto>(() =>
                _logService.GetLogsAsync(new GetLogsRequestDto
                {
                    ApplicationId = getLogsToolArg.applicationId,
                    Level = getLogsToolArg.level,
                    From = getLogsToolArg.from,
                    To = getLogsToolArg.to,
                    RequestPath = getLogsToolArg.requestPath,
                    RequestMethod = getLogsToolArg.requestMethod,
                    StatusCode = getLogsToolArg.statusCode,
                    TraceId = getLogsToolArg.traceId,
                    Page = getLogsToolArg.page,
                    PageSize = getLogsToolArg.pageSize
                }), Name);
        }
    }

    public class GetLogsToolArg()
    {
        public Guid applicationId { get; set; }
        public eLogLevel? level { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public string? requestPath { get; set; }
        public string? requestMethod { get; set; }
        public int? statusCode { get; set; }
        public string? traceId { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 50;
    }
}
