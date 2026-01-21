using Api.Hubs;
using Api.Services;
using Application.Interfaces;
using Application.UseCases;
using Infra.Cache;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// postgresql connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TicketDbContext>(options => options.UseNpgsql(connectionString));

// redis connection
var redisString = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisString!));

// dependency injections
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatCache, SeatCache>();
builder.Services.AddSignalR();
builder.Services.AddScoped<ITicketNotifier, TicketNotifier>();
builder.Services.AddScoped<HoldSeatUseCase>();

// swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TicketMaster Api", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// signalR hub
app.MapHub<TicketHub>("/ticketHub");

// database initialization and seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<TicketDbContext>();
        
        // ensure database schema is created
        context.Database.EnsureCreated();
        
        // seed initial data if empty
        await DbSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"error seeding database: {ex.Message}");
    }
}

app.Run();