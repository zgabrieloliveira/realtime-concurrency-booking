namespace Application.Interfaces;

public interface ISeatCache
{
    Task<bool> LockSeatAsync(string seatId, string userId, TimeSpan ttl);
}