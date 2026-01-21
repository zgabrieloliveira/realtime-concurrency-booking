using Domain.Entities;

namespace Infra.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(TicketDbContext context)
    {
        // check if database is already populated to avoid duplication
        if (context.Seats.Any()) 
            return;

        var seats = new List<Seat>();
        var rows = new[] { "A", "B", "C", "D", "E" };

        // generate 20 seats for each row (100 seats total)
        foreach (var row in rows)
        {
            for (var number = 1; number <= 20; number++)
            {
                seats.Add(new Seat(row, number));
            }
        }

        // use addrangeasync for better performance with bulk inserts
        await context.Seats.AddRangeAsync(seats);
        
        // commit changes to the database
        await context.SaveChangesAsync();
    }
}