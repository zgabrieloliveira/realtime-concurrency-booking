using Application.Interfaces;
using StackExchange.Redis;

namespace Infra.Cache;

public class SeatCache : ISeatCache
{
    private readonly IDatabase _redis;

    public SeatCache(IConnectionMultiplexer muxer)
    {
        _redis = muxer.GetDatabase();
    }

    public async Task<bool> LockSeatAsync(string seatId, string userId, TimeSpan ttl)
    {
        var key = $"seat:{seatId}";
        var value = userId;
        return await _redis.StringSetAsync(key, value, ttl, When.NotExists);
    }
}