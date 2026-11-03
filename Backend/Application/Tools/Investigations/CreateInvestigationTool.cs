using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;
using System.Text.Json;

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
                    Title = createInvestigationToolArg.title,
                    Summary = createInvestigationToolArg.summary,
                    RootCause = createInvestigationToolArg.rootCause,
                    Recommendations = createInvestigationToolArg.recommendations,
                    Confidence = createInvestigationToolArg.confidence,
                    ModelName = createInvestigationToolArg.modelName
                }), Name);
        }
    }

    public class CreateInvestigationToolArg()
    {
        public Guid applicationId { get; set; }
        public string title { get; set; } = null!;
        public string? summary { get; set; }
        public string? rootCause { get; set; }
        public JsonElement? recommendations { get; set; }
        public double? confidence { get; set; }
        public string? modelName { get; set; }
    }
}
