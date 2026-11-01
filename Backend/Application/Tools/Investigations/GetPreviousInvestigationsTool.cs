using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;
using Domain.Enums;

namespace Application.Tools.Investigations
{
    public class GetPreviousInvestigationsTool : BaseTool, IAgentTool
    {
        private readonly AIInvestigationService _investigationService;

        public GetPreviousInvestigationsTool(AIInvestigationService investigationService)
        {
            _investigationService = investigationService;
        }

        public string Name => "GetPreviousInvestigationsTool";
        public Type ArgType => typeof(GetPreviousInvestigationsToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetPreviousInvestigationsToolArg getPreviousInvestigationsToolArg = (GetPreviousInvestigationsToolArg)arg;

            return await ExecuteTool<GetPreviousInvestigationsResponseDto>(() =>
                _investigationService.GetPreviousInvestigationsAsync(new GetPreviousInvestigationsRequestDto
                {
                    ApplicationId = getPreviousInvestigationsToolArg.applicationId,
                    From = getPreviousInvestigationsToolArg.from,
                    To = getPreviousInvestigationsToolArg.to,
                    Status = getPreviousInvestigationsToolArg.status,
                    Page = getPreviousInvestigationsToolArg.page,
                    PageSize = getPreviousInvestigationsToolArg.pageSize
                }), Name);
        }
    }

    public class GetPreviousInvestigationsToolArg()
    {
        public Guid applicationId { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public eInvestigationStatus? status { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
    }
}
