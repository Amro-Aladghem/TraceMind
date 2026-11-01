using Application.Tools.Interfaces;
using System.Text.Json;

namespace Application.Tools
{
    public class ToolExecutor
    {
        private readonly IEnumerable<IAgentTool> _agentTools;
        private readonly Dictionary<string, ToolExecutorModel> _toolsDictionary;

        public ToolExecutor(IEnumerable<IAgentTool> agentTools)
        {
            _agentTools = agentTools;
            _toolsDictionary = GetToolsDictionary();
        }

        private Dictionary<string, ToolExecutorModel> GetToolsDictionary()
        {
            Dictionary<string, ToolExecutorModel> dict = new Dictionary<string, ToolExecutorModel>();

            foreach (IAgentTool agentTool in _agentTools)
            {
                dict.Add(agentTool.Name, new ToolExecutorModel()
                {
                    ArgType = agentTool.ArgType,
                    Execute = agentTool.ExecuteAsync
                });
            }

            return dict;
        }

        private object TryToGetArgument(JsonElement data, Type argType, string toolName)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var argument = JsonSerializer.Deserialize(data.GetRawText(), argType, options);
                return argument!;
            }
            catch
            {
                throw new Exception($"Your Sending Argument for tool:${toolName} has an issue please check your instruction and try again");
            }
        }

        public async Task<ToolResponse> HandleExecuteTool(string toolName, JsonElement data)
        {
            try
            {
                if (!_toolsDictionary.TryGetValue(toolName, out var toolExecutorModel))
                    throw new Exception($"Tool with name {toolName} is not exists,please check the correct name and try again");

                object argument = data.ValueKind != JsonValueKind.Undefined
                    ? TryToGetArgument(data, toolExecutorModel.ArgType, toolName)
                    : new object { };

                ToolResponse response = await toolExecutorModel.Execute(argument);

                return response;
            }
            catch (Exception ex)
            {
                return new ToolResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }

    public class ToolExecutorModel()
    {
        public Type ArgType { get; set; } = null!;
        public Func<object, Task<ToolResponse>> Execute { get; set; } = null!;
    }
}
