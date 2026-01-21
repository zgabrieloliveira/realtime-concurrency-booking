using Domain.Enums;

namespace Domain.Entities;

public class Seat
{
    public Guid Id { get; private set; }
    public string Row { get; private set; } = string.Empty;
    public int Number { get; private set; }
    public SeatStatus Status { get; private set; }
    public string? CurrentUserId { get; private set; }

    protected Seat()
    {
    }
    
    public Seat(string row, int number)
    {
        Id = Guid.NewGuid();
        Row = row;
        Number = number;
        Status = SeatStatus.Available;
    }
    
    // only available seats can be locked
    public void Lock(string userId)
    {
        if (Status != SeatStatus.Available)
            throw new InvalidOperationException($"Seat {Row}-{Number} is not available.");

        Status = SeatStatus.Locked;
        CurrentUserId = userId;
    }

    // only locked seats can be sold
    public void Sell()
    {
        if (Status != SeatStatus.Locked)
            throw new InvalidOperationException($"Seat {Row}-{Number} must be locked before selling.");
        
        Status = SeatStatus.Sold;
    }
    
    // seat reservation expired or cancelled
    public void Release()
    {
        Status = SeatStatus.Available;
        CurrentUserId = null;
    }

}