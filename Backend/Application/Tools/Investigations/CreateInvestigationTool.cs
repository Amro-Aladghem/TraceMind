using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;

namespace Application.Tools.Investigations
{
    public class CreateInvestigationTool : BaseTool, IAgentTool
    {
        private readonly AIInvestigationService _investigationService;

        public CreateInvestigationTool(AIInvestigationService investigationService)
        {
            _investigationService = investigationService;
        }

        public string Name => "CreateInvestigationTool";
        public Type ArgType => typeof(CreateInvestigationToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            CreateInvestigationToolArg createInvestigationToolArg = (CreateInvestigationToolArg)arg;

            return await ExecuteTool<CreateInvestigationResponseDto>(() =>
                _investigationService.CreateInvestigationAsync(new CreateInvestigationRequestDto
                {
                    ApplicationId = createInvestigationToolArg.applicationId,
                    Title = createInvestigationToolArg.title
                }), Name);
        }
    }

    public class CreateInvestigationToolArg()
    {
        public Guid applicationId { get; set; }
        public string title { get; set; } = null!;
    }
}
