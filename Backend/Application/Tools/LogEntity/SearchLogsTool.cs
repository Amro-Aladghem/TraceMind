using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;
using Domain.Enums;

namespace Application.Tools.Logs
{
    public class SearchLogsTool : BaseTool, IAgentTool
    {
        private readonly LogService _logService;

        public SearchLogsTool(LogService logService)
        {
            _logService = logService;
        }

        public string Name => "SearchLogsTool";
        public Type ArgType => typeof(SearchLogsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            SearchLogsToolArg searchLogsToolArg = (SearchLogsToolArg)arg;

            return await ExecuteTool<SearchLogsResponseDto>(() =>
                _logService.SearchLogsAsync(new SearchLogsRequestDto
                {
                    ApplicationId = searchLogsToolArg.applicationId,
                    SearchTerm = searchLogsToolArg.searchTerm,
                    From = searchLogsToolArg.from,
                    To = searchLogsToolArg.to,
                    Level = searchLogsToolArg.level,
                    Limit = searchLogsToolArg.limit
                }), Name);
        }
    }

    public class SearchLogsToolArg()
    {
        public Guid applicationId { get; set; }
        public string searchTerm { get; set; } = null!;
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public eLogLevel? level { get; set; }
        public int limit { get; set; } = 50;
    }
}
