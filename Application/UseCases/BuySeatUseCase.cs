using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases;

public class BuySeatUseCase
{
    private readonly ISeatRepository _repository;
    private readonly ISeatCache _cache;
    private readonly ITicketNotifier _notifier;

    public BuySeatUseCase(
        ISeatRepository repository, 
        ISeatCache cache, 
        ITicketNotifier notifier)
    {
        _repository = repository;
        _cache = cache;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(BuySeatRequestDto request)
    {
        // 1. verify lock ownership in redis
        var lockerUser = await _cache.GetLockerAsync(request.SeatId.ToString());
        
        if (string.IsNullOrEmpty(lockerUser))
            throw new Exception("Reservation expired or not found! Please hold the seat again.");
            
        if (lockerUser != request.UserId)
            throw new Exception("Seat is reserved by another user.");

        // 2. get seat from database
        var seat = await _repository.GetByIdAsync(request.SeatId);
        if (seat is null) throw new Exception("Seat not found.");

        // on postgres side, if seat is still available,
        // lock it for this user
        if (seat.Status == SeatStatus.Available)
        {
            seat.Lock(request.UserId);
        }
        
        // 3. execute domain logic (change status to Sold)
        seat.Sell(); 

        // 4. persist changes to postgres
        await _repository.UpdateAsync(seat);

        // 5. remove lock from redis (no longer needed since it's permanently sold)
        await _cache.RemoveLockAsync(request.SeatId.ToString());

        // 6. notify listeners
        await _notifier.NotifySeatSoldAsync(request.SeatId.ToString());
    }
}