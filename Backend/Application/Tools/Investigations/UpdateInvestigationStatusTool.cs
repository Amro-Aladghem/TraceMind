using Application.Dtos;
using Application.Services;
using Application.Tools.Interfaces;
using Domain.Enums;

namespace Application.Tools.Investigations
{
    public class UpdateInvestigationStatusTool : BaseTool, IAgentTool
    {
        private readonly AIInvestigationService _investigationService;

        public UpdateInvestigationStatusTool(AIInvestigationService investigationService)
        {
            _investigationService = investigationService;
        }

        public string Name => "UpdateInvestigationStatusTool";
        public Type ArgType => typeof(UpdateInvestigationStatusToolArg);

        public async Task<ToolResponse> ExecuteAsync(object arg)
        {
            UpdateInvestigationStatusToolArg updateInvestigationStatusToolArg = (UpdateInvestigationStatusToolArg)arg;

            return await ExecuteTool<UpdateInvestigationStatusResponseDto>(() =>
                _investigationService.UpdateInvestigationStatusAsync(new UpdateInvestigationStatusRequestDto
                {
                    InvestigationId = updateInvestigationStatusToolArg.investigationId,
                    Status = updateInvestigationStatusToolArg.status
                }), Name);
        }
    }

    public class UpdateInvestigationStatusToolArg()
    {
        public Guid investigationId { get; set; }
        public eInvestigationStatus status { get; set; }
    }
}
