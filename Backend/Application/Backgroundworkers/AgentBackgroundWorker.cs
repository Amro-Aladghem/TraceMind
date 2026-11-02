
using Application.Dtos;
using Application.Services;
using Application.Services.Coordinators;
using Domain.Enities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace Infrastructure.Backgroundworkers
{
    public class AgentBackgroundWorker : BackgroundService
    {
        private readonly Channel<AgentRequestDto> _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        public AgentBackgroundWorker(
            Channel<AgentRequestDto> channel,
            IServiceScopeFactory scopeFactory)
        {
            _channel = channel;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await foreach (
                var job in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await using var scope =
                        _scopeFactory.CreateAsyncScope();

                    var executionService = scope.ServiceProvider
                        .GetRequiredService<AgentExecutionService>();

                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<NotificationService>();

                    await notificationService.CreateNotification(job.StoreId, new NotificationDto()
                    {
                        ChatSignature = job.ChatSignture!.Value,
                        AgentType = job.AgentType!,
                        NotificationStatus = eNotificationStatus.Running.ToString(),
                    });

                    await executionService.ExecutAgent(
                        job,
                        job.AgentType!);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                }
            }
        }
    }
}
