using StackExchange.Redis;
namespace likhitan.Common.Services;

//public class RedisCleanupService : BackgroundService
//{
//    private readonly IConnectionMultiplexer _redis;

//    public RedisCleanupService(IConnectionMultiplexer redis)
//    {
//        _redis = redis;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            var server = _redis.GetServer(_redis.GetEndPoints()[0]);

//            await Task.Run(() => server.FlushDatabase());

//            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
//        }
//    }
//}
