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
            client.BaseAddress = new Uri(configuration["ReqResApi:BaseUrl"]??"");
            client.DefaultRequestHeaders.Add("x-api-key", configuration["ReqResApi:ApiKey"]);
        });
        services.AddMemoryCache();
    })
    .Build();


// Get All Records form the ("https://reqres.in/api/users?page={pageNumber}")

var userService = host.Services.GetRequiredService<IExternalUserService>();
var users = await userService.GetAllUsersAsync();

foreach (var user in users)
{
    Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName}");
}

// Get Single Records form the ("https://reqres.in/api/users/{userId}")
var SingleUser = await userService.GetUserByIdAsync(1);

Console.WriteLine("Id :"+SingleUser.Id );
Console.WriteLine("Email :"+SingleUser.Email );
Console.WriteLine("FirstName :"+SingleUser.FirstName );
Console.WriteLine("LastName :"+SingleUser.LastName );
Console.WriteLine("Avatar :" + SingleUser.Avatar);