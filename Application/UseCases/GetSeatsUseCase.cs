using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases;

public class GetSeatsUseCase
{
    private readonly ISeatRepository _repository;
    private readonly ISeatCache _cache;

    public GetSeatsUseCase(ISeatRepository repository, ISeatCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<SeatDto>> ExecuteAsync()
    {
        // searches on the database for all seats
        var seats = await _repository.GetAllAsync();

        // searches on redis for temporary locks
        var lockedIdsInRedis = await _cache.GetAllLockedSeatIdsAsync();

        var lockedSet = lockedIdsInRedis.ToHashSet(); // O(1) lookups
        
        return seats.Select(s => 
        {
            var status = s.Status.ToString();
            
            // available seats that are locked in redis are shown as "Locked" to the user
            if (s.Status == SeatStatus.Available && lockedSet.Contains(s.Id.ToString()))
            {
                status = "Locked";
            }

            return new SeatDto(s.Id, s.Row, s.Number, status);
        });
    }
}