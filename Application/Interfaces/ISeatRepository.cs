using Domain.Entities;

namespace Application.Interfaces;

public interface ISeatRepository
{
    Task<Seat?> GetByIdAsync(Guid id);
    Task UpdateAsync(Seat seat);
}