using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Investigations
{
    public class GetInvestigationByIdTool : BaseTool, IAgentTool
    {
        private readonly AIInvestigationService _investigationService;

        public GetInvestigationByIdTool(AIInvestigationService investigationService)
        {
            _investigationService = investigationService;
        }

        public string Name => "GetInvestigationByIdTool";
        public Type ArgType => typeof(GetInvestigationByIdToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            GetInvestigationByIdToolArg getInvestigationByIdToolArg = (GetInvestigationByIdToolArg)arg;

            return await ExecuteTool<GetInvestigationByIdResponseDto>(async () =>
            {
                return await _investigationService.GetInvestigationByIdAsync(new GetInvestigationByIdRequestDto
                {
                    InvestigationId = getInvestigationByIdToolArg.investigationId
                }) ?? throw new InvalidOperationException("Investigation was not found");
            }, Name);
        }
    }

    public class GetInvestigationByIdToolArg()
    {
        public Guid investigationId { get; set; }
    }
}
