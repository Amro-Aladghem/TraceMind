using Application.Dtos;
using Application.Tools;
using Application.Tools.Interfaces;
using Infrastructure.Backgroundworkers;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using Presentation;
using System.Reflection;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(context =>
    context.UseNpgsql(builder.Configuration["SQL_CONNECTION_STRING"]));

builder.Services.AddSingleton(new OllamaApiClient(new Uri(
    builder.Configuration["OllamaUrl"] ?? "http://localhost:11434")));

builder.Services.AddSingleton<Channel<AgentRequestDto>>(
    _ => Channel.CreateUnbounded<AgentRequestDto>()
);

var applicationAssembly = typeof(IAgentTool).Assembly;
var infrastructureAssembly = typeof(AppDbContext).Assembly;

var serviceNamespaces = new (Assembly Assembly, string Namespace)[]
{
    (applicationAssembly, "Application.Services"),
    (applicationAssembly, "Application.Services.Coordinators"),
    (applicationAssembly, "Application.Agents"),
    (applicationAssembly, "Application.Tools.Coordinator"),
    (infrastructureAssembly, "Infrastructure.AI.Chat"),
};

foreach (var (assembly, targetNamespace) in serviceNamespaces)
{
    var serviceTypes = assembly.GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.Namespace == targetNamespace);

    foreach (var serviceType in serviceTypes)
        builder.Services.AddScoped(serviceType);
}

var toolTypes = applicationAssembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && typeof(IAgentTool).IsAssignableFrom(type));

foreach (var toolType in toolTypes)
{
    builder.Services.AddScoped(toolType);
    builder.Services.AddScoped(typeof(IAgentTool), provider => provider.GetRequiredService(toolType));
}

builder.Services.AddScoped<ToolExecutor>();
builder.Services.AddHostedService<AgentBackgroundWorker>();

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AssemblyReference).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
