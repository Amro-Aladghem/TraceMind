using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;
using System.Text.Json;

namespace Application.Tools.Investigations
{
    public class UpdateInvestigationTool : BaseTool, IAgentTool
    {
        private readonly AIInvestigationService _investigationService;

        public UpdateInvestigationTool(AIInvestigationService investigationService)
        {
            _investigationService = investigationService;
        }

        public string Name => "UpdateInvestigationTool";
        public Type ArgType => typeof(UpdateInvestigationToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            UpdateInvestigationToolArg updateInvestigationToolArg = (UpdateInvestigationToolArg)arg;

            return await ExecuteTool<UpdateInvestigationResponseDto>(() =>
                _investigationService.UpdateInvestigationAsync(new UpdateInvestigationRequestDto
                {
                    InvestigationId = updateInvestigationToolArg.investigationId,
                    Title = updateInvestigationToolArg.title,
                    Summary = updateInvestigationToolArg.summary,
                    RootCause = updateInvestigationToolArg.rootCause,
                    Recommendations = updateInvestigationToolArg.recommendations,
                    ModelName = updateInvestigationToolArg.modelName
                }), Name);
        }
    }

    public class UpdateInvestigationToolArg()
    {
        public Guid investigationId { get; set; }
        public string title { get; set; } = null!;
        public string summary { get; set; } = null!;
        public string rootCause { get; set; } = null!;
        public JsonElement? recommendations { get; set; }
        public string? modelName { get; set; }
    }
}
