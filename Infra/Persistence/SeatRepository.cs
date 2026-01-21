using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence;

public class SeatRepository : ISeatRepository
{
    private readonly TicketDbContext _context;
    
    public SeatRepository(TicketDbContext context)
    {
        _context = context;
    }
    
    public async Task<Seat?> GetByIdAsync(Guid id)
    {
        return await _context.Seats.FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Seat>> GetAllAsync()
    {
        // AsNoTracking for read-only queries to improve performance
        return await _context.Seats.AsNoTracking().ToListAsync();
    }
    
}