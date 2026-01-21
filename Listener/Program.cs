using Microsoft.AspNetCore.SignalR.Client;

// setup console visuals
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.White;
Console.Clear();
Console.WriteLine("--- TICKETS REAL-TIME LISTENER ---");
Console.ResetColor();

// get url from environment variable (docker) or use default (local)
var baseUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000";
var url = $"{baseUrl}/ticketHub";

// build the connection
var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .WithAutomaticReconnect() // auto-reconnect if connection is lost
    .Build();

// register the handler for the 'ReceiveSeatUpdate' event
// matches the method name defined in Api/Services/SignalRNotifier.cs
connection.On<string, string>("ReceiveSeatUpdate", (seatId, status) =>
{
    var color = status == "LOCKED" ? ConsoleColor.Yellow : ConsoleColor.Green;
    
    Console.ForegroundColor = color;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ALERT: seat {seatId} status changed to: {status}");
    Console.ResetColor();
});

// start the connection
try 
{
    Console.WriteLine($"connecting to {url}...");
    await connection.StartAsync();
    Console.WriteLine("connected successfully! waiting for events...");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"error connecting: {ex.Message}");
    Console.WriteLine("check if the api is running and the port is correct.");
    Console.ResetColor();
}

// keep the app running
Console.WriteLine("\npress ENTER to exit...");
Console.ReadLine();