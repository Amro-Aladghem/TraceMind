using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetDailyErrorLogsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetDailyErrorLogsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetDailyErrorLogsTool";
        public Type ArgType => typeof(GetDailyErrorLogsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetDailyErrorLogsToolArg getDailyErrorLogsToolArg = (GetDailyErrorLogsToolArg)arg;

            return await ExecuteTool<List<DailyErrorLogDto>>(() =>
                _logService.GetDailyErrorLogsAsync(getDailyErrorLogsToolArg.applicationId), Name);
        }
    }

    public class GetDailyErrorLogsToolArg()
    {
        public Guid applicationId { get; set; }
    }
}
