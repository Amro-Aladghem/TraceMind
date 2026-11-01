using Application.Tools;
using Application.Tools.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(context =>
    context.UseNpgsql(builder.Configuration["SQL_CONNECTION_STRING"]));

var applicationAssembly = typeof(IAgentTool).Assembly;

var serviceTypes = applicationAssembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && type.Namespace == "Application.Services");

foreach (var serviceType in serviceTypes)
    builder.Services.AddScoped(serviceType);

var toolTypes = applicationAssembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && typeof(IAgentTool).IsAssignableFrom(type));

foreach (var toolType in toolTypes)
{
    builder.Services.AddScoped(toolType);
    builder.Services.AddScoped(typeof(IAgentTool), provider => provider.GetRequiredService(toolType));
}

builder.Services.AddScoped<ToolExecutor>();

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
