using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetRequestStatisticsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetRequestStatisticsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetRequestStatisticsTool";
        public Type ArgType => typeof(GetRequestStatisticsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetRequestStatisticsToolArg getRequestStatisticsToolArg = (GetRequestStatisticsToolArg)arg;

            return await ExecuteTool<GetRequestStatisticsResponseDto>(() =>
                _logService.GetRequestStatisticsAsync(new GetRequestStatisticsRequestDto
                {
                    ApplicationId = getRequestStatisticsToolArg.applicationId,
                    From = getRequestStatisticsToolArg.from,
                    To = getRequestStatisticsToolArg.to
                }), Name);
        }
    }

    public class GetRequestStatisticsToolArg()
    {
        public Guid applicationId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
