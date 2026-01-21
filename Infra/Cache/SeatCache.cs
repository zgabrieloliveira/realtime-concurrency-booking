using Application.Interfaces;
using StackExchange.Redis;

namespace Infra.Cache;

public class SeatCache : ISeatCache
{
    private readonly IDatabase _redis;
    private readonly IConnectionMultiplexer _muxer;

    public SeatCache(IConnectionMultiplexer muxer)
    {
        _muxer = muxer;
        _redis = muxer.GetDatabase();
    }

    public async Task<bool> LockSeatAsync(string seatId, string userId, TimeSpan ttl)
    {
        var key = $"seat:{seatId}";
        var value = userId;
        return await _redis.StringSetAsync(key, value, ttl, When.NotExists);
    }
    
    public async Task<string?> GetLockerAsync(string seatId)
    {
        return await _redis.StringGetAsync($"seat:{seatId}");
    }
    
    public async Task RemoveLockAsync(string seatId)
    {
        await _redis.KeyDeleteAsync($"seat:{seatId}");
    }
    
    public Task<IEnumerable<string>> GetAllLockedSeatIdsAsync()
    {
        // get the server to perform key search
        var endpoint = _muxer.GetEndPoints().First();
        var server = _muxer.GetServer(endpoint);
        // get all keys matching the pattern "seat:*"
        var keys = server.Keys(pattern: "seat:*");
        // clean up the keys to return only seat IDs
        var result = keys.Select(k => k.ToString().Replace("seat:", "")).ToList();
        return Task.FromResult<IEnumerable<string>>(result);
    }
}