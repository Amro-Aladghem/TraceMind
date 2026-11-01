using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetMostFrequentErrorsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetMostFrequentErrorsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetMostFrequentErrorsTool";
        public Type ArgType => typeof(GetMostFrequentErrorsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetMostFrequentErrorsToolArg getMostFrequentErrorsToolArg = (GetMostFrequentErrorsToolArg)arg;

            return await ExecuteTool<GetMostFrequentErrorsResponseDto>(() =>
                _logService.GetMostFrequentErrorsAsync(new GetMostFrequentErrorsRequestDto
                {
                    ApplicationId = getMostFrequentErrorsToolArg.applicationId,
                    From = getMostFrequentErrorsToolArg.from,
                    To = getMostFrequentErrorsToolArg.to,
                    Limit = getMostFrequentErrorsToolArg.limit
                }), Name);
        }
    }

    public class GetMostFrequentErrorsToolArg()
    {
        public Guid applicationId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int limit { get; set; } = 10;
    }
}
