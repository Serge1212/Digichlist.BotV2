using Digichlist.Bot.Commands;
using Digichlist.Bot.Configuration;
using Digichlist.Bot.Context;
using Digichlist.Bot.Handlers;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = new HostBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(cfg => cfg.AddConsole());
        var botToken = Environment.GetEnvironmentVariable("DigichlistV2", EnvironmentVariableTarget.User) ?? throw new ArgumentException("Missing API token for the Telegram Bot.");
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
        services.AddSingleton<IUpdateHandler, BotHandler>();
        // Commands.
        services.AddTransient<StartCommand>();
        services.AddTransient<RegisterMeCommand>();
        // Repos.
        services.AddTransient<IUserRepository, UserRepository>();

        services.AddDbContext<DigichlistContext>(options =>
        {
            var server = Environment.GetEnvironmentVariable("DIGICHLIST_SERVER", EnvironmentVariableTarget.User);
            var database = Environment.GetEnvironmentVariable("DIGICHLIST_DATABASE", EnvironmentVariableTarget.User);
            var username = Environment.GetEnvironmentVariable("DIGICHLIST_DB_USERNAME", EnvironmentVariableTarget.User);
            var password = Environment.GetEnvironmentVariable("DIGICHLIST_DB_PASSWORD", EnvironmentVariableTarget.User);
            var template = Environment.GetEnvironmentVariable("DIGICHLIST_DB_CONNECTION_TEMPLATE", EnvironmentVariableTarget.User);

            var conString = string.Format(template!,
                server,
                database,
                username,
                password);

            options.UseSqlServer(conString, ops =>
            {
                ops
                 .CommandTimeout(30)
                 .EnableRetryOnFailure();
            });
        });

        // Explicit override of db context lifetime.
        // It's necessary for having the freshest db info per each request.
        services.AddTransient<DigichlistContext>();
    });

var host = builder.Build();

var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;

// Launching our bot.
try
{
    var botClient = services.GetService<ITelegramBotClient>();
    var updateHandler = services.GetService<IUpdateHandler>();

    if (botClient is null || updateHandler is null)
    {
        throw new InvalidOperationException("Could not configure the bot handlers.");
    }

    botClient.StartReceiving(updateHandler);

    Console.WriteLine($"{Constants.APP_NAME} is all set.");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"Error Occured: {ex}");
}