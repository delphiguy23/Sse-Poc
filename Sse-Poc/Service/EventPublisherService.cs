namespace Sse_Poc.Service;

public class EventPublisherService(SseService sseService, ILogger<EventPublisherService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var myEvent = new
                {
                    Message = $"Periodic Event at {DateTime.UtcNow}",
                    Value = new Random().Next(1, 100)
                };

                sseService.PublishEvent("test", myEvent);
                logger.LogInformation($"Published event: {myEvent.Message}");

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
            }
        }
    }
}
