using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaftLabsTest.Application;
using RaftLabsTest.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddHttpClient<IExternalUserService, ExternalUserService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ReqResApi:BaseUrl"]);
        });

        // Optional: Add in-memory cache
        services.AddMemoryCache();
    })
    .Build();

// Resolve and use the service
var service = host.Services.GetRequiredService<IExternalUserService>();
var users = await service.GetAllUsersAsync();

foreach (var user in users)
{
    Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName}");
}
