using ChannelEngine.Business.Clients;
using ChannelEngine.Business.Services;
using ChannelEngine.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Services
services.AddHttpClient<IChannelEngineClient, ChannelEngineClient>();
services.AddTransient<IProductService, ProductService>();
services.AddTransient<JobTop5Products>();
using var provider = services.BuildServiceProvider();

// Resolve job
var job = provider.GetRequiredService<JobTop5Products>();

// Graceful shutdown handling
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nCancellation requested...");
};

// Run the job
await job.StartAsync(cts.Token);

Console.WriteLine("Job finished. Press any key to exit.");
Console.ReadKey();