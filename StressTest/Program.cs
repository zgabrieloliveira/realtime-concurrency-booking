using System.Net.Http.Json;
using System.Diagnostics;

// settings
var apiUrl = "http://localhost:5000/Api/Seats/Hold";
var seatId = "44e08391-059d-4b5e-a3f3-f723bd3dcdd0"; // can be any available seat id
var totalRequests = 50;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"--- Starting Stress Test: {totalRequests} concurrent requests ---");
Console.ResetColor();

var client = new HttpClient();
var tasks = new List<Task<HttpResponseMessage>>();
var successCount = 0;
var failCount = 0;

var stopwatch = Stopwatch.StartNew();

// fire 50 requests without waiting for the response (initial fire and forget)
for (int i = 0; i < totalRequests; i++)
{
    var userId = $"user-bot-{i}";
    var payload = new { seatId, userId };

    // add task to list
    tasks.Add(client.PostAsJsonAsync(apiUrl, payload));
}

// wait for all to finish together
var responses = await Task.WhenAll(tasks);
stopwatch.Stop();

// analyze results
foreach (var response in responses)
{
    if (response.IsSuccessStatusCode)
    {
        Interlocked.Increment(ref successCount);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Success! (status: {response.StatusCode})");
    }
    else
    {
        Interlocked.Increment(ref failCount);
        // show error message from api
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"fail: {error}"); 
    }
}

Console.ResetColor();
Console.WriteLine("\n" + new string('-', 30));
Console.WriteLine($"TOTAL TIME: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"SUCCESSES (LOCKS): {successCount} (EXPECTED: 1)");
Console.WriteLine($"BLOCKED (PROTECTED): {failCount} (EXPECTED: {totalRequests - 1})");
Console.WriteLine(new string('-', 30));

if (successCount == 1 && failCount == totalRequests - 1)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nSystem approved the test! Only one user managed to hold the seat.");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nTest failed! More than one user managed to hold or no one succeeded.");
}

Console.ResetColor();