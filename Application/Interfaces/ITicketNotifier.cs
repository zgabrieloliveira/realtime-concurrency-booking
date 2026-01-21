namespace Application.Interfaces;

public interface ITicketNotifier
{
    Task NotifySeatLockedAsync(string seatId);
}