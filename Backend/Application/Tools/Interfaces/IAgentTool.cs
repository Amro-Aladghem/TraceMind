namespace Application.Tools.Interfaces
{
    public interface IAgentTool
    {
        public string Name { get; }
        public Type ArgType { get; }
        public Task<ToolResponse> ExecuteAsync(object arg);
    }
}
