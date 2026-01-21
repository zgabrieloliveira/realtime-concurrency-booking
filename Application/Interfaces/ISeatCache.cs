namespace Application.Interfaces;

public interface ISeatCache
{
    Task<bool> LockSeatAsync(string seatId, string userId, TimeSpan ttl);
    Task<string?> GetLockerAsync(string seatId);
    Task RemoveLockAsync(string seatId);
    Task<IEnumerable<string>> GetAllLockedSeatIdsAsync();
}