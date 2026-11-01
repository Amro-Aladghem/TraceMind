using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Logs
{
    public class GetErrorCountTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public GetErrorCountTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "GetErrorCountTool";
        public Type ArgType => typeof(GetErrorCountToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetErrorCountToolArg getErrorCountToolArg = (GetErrorCountToolArg)arg;

            return await ExecuteTool<GetErrorCountResponseDto>(() =>
                _logService.GetErrorCountAsync(new GetErrorCountRequestDto
                {
                    ApplicationId = getErrorCountToolArg.applicationId,
                    From = getErrorCountToolArg.from,
                    To = getErrorCountToolArg.to
                }), Name);
        }
    }

    public class GetErrorCountToolArg()
    {
        public Guid applicationId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
