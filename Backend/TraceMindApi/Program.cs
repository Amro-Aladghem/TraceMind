using Application.Agents;
using Application.Dtos;
using Application.Services;
using Application.Services.Coordinators;
using Application.Tools;
using Application.Tools.Coordinator;
using Application.Tools.Interfaces;
using Infrastructure.AI.Chat;
using Infrastructure.AI.Interfaces;
using Infrastructure.Backgroundworkers;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using Presentation;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(context =>
    context.UseNpgsql(builder.Configuration["SQL_CONNECTION_STRING"]));

var httpClient = new HttpClient
{
    BaseAddress = new Uri(
        builder.Configuration.GetSection("Ollama_Uri").Value!),
    Timeout = TimeSpan.FromMinutes(10)
};

builder.Services.AddSingleton(new OllamaApiClient(httpClient));

builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<AIInvestigationService>();

builder.Services.AddScoped<AgentChatCoordinator>();
builder.Services.AddScoped<AgentExecutionService>();
builder.Services.AddScoped<ToolChatCoordinator>();


builder.Services.AddScoped<LoggingAgent>();
builder.Services.AddScoped<LoggingAgentChat>();
builder.Services.AddScoped<IAgentChat>(provider => provider.GetRequiredService<LoggingAgentChat>());

var toolTypes = typeof(IAgentTool).Assembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && typeof(IAgentTool).IsAssignableFrom(type));

foreach (var toolType in toolTypes)
{
    builder.Services.AddScoped(toolType);
    builder.Services.AddScoped(typeof(IAgentTool), provider => provider.GetRequiredService(toolType));
}

builder.Services.AddScoped<ToolExecutor>();


builder.Services.AddSingleton<Channel<AgentRequestDto>>(
    _ => Channel.CreateUnbounded<AgentRequestDto>()
);

builder.Services.AddHostedService<AgentBackgroundWorker>();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AssemblyReference).Assembly);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
