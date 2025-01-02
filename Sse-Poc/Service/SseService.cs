using System.Text.Json;
using System.Threading.Channels;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Sse_Poc.Service;

public class SseService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ConcurrentDictionary<string, Channel<string>> _channels = new();

    public SseService(string redisConnectionString)
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { "localhost:6379" },
            ConnectRetry = 3,
            ConnectTimeout = 2000,
            AbortOnConnectFail = false
        };
        _redis = ConnectionMultiplexer.Connect(options);
    }

    public async Task SubscribeToChannelAsync(string channel, ChannelWriter<string> writer, CancellationToken cancellationToken)
    {
        var subscriber = _redis.GetSubscriber();

        subscriber.Subscribe(channel, (ch, message) =>
        {
            try 
            {
                writer.TryWrite(message);
            }
            catch (Exception ex)
            {
                // Log error
            }
        });

        // Keep the subscription alive
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    public void PublishEvent<T>(string channel, T eventData)
    {
        var subscriber = _redis.GetSubscriber();
        var message = JsonSerializer.Serialize(eventData);
        subscriber.Publish(channel, message);
    }
}
