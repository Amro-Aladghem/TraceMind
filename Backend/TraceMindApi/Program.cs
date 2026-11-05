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
using System.Text.Json.Serialization;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(context =>
    context.UseNpgsql(builder.Configuration["SQL_CONNECTION_STRING"]));

var ollamaUri = builder.Configuration["Ollama_Uri"]
    ?? Environment.GetEnvironmentVariable("OLLAMA_URL")
    ?? builder.Configuration["OllamaUrl"]
    ?? "http://localhost:11434";

var httpClient = new HttpClient
{
    BaseAddress = new Uri(ollamaUri),
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173", "http://127.0.0.1:5173",
                  "http://localhost:5174", "http://127.0.0.1:5174")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddApplicationPart(typeof(AssemblyReference).Assembly);

var app = builder.Build();

app.UseCors("Frontend");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
