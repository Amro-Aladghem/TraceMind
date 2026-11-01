using System.Text.Json;

namespace Application.Tools
{
    public abstract class BaseTool
    {
        protected async Task<ToolResponse> ExecuteTool<T>(Func<Task<T>> action, string toolName)
        {
            try
            {
                var result = await action();

                return new ToolResponse()
                {
                    Success = true,
                    Message = $"The Tool {toolName} execution successed, you can continue your workflow",
                    Data = JsonSerializer.Serialize<T>(result)
                };
            }
            catch (Exception)
            {
                return new ToolResponse()
                {
                    Success = false,
                    Message = $"The Tool {toolName} execution failed. You may retry once only"
                };
            }
        }
    }
}
