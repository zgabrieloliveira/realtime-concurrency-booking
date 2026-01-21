using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases;

public class HoldSeatUseCase
{
    private readonly ISeatRepository _repository;
    private readonly ISeatCache _cache;
    private readonly ITicketNotifier _notifier;

    public HoldSeatUseCase(
        ISeatRepository repository,
        ISeatCache cache,
        ITicketNotifier notifier)
    {
        _repository = repository;
        _cache = cache;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(HoldSeatRequestDto request)
    {
        var seat = await _repository.GetByIdAsync(request.SeatId);
        
        if (seat is null) throw new Exception("Seat not found");
        if (seat.Status == SeatStatus.Sold) throw new Exception("Seat already sold");

        var acquired = await _cache
            .LockSeatAsync(
                request.SeatId.ToString(),
                request.UserId,
                TimeSpan.FromMinutes(10));
        
        if (!acquired) throw new Exception("Seat is currently held by another user.");

        await _notifier.NotifySeatLockedAsync(request.SeatId.ToString());
    }

}