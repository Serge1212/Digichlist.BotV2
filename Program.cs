using Digichlist.Bot.Commands;
using Digichlist.Bot.Configuration;
using Digichlist.Bot.Handlers;
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
    });

var host = builder.Build();

var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;

// Launching our bot.
try
{
    var botClient = services.GetService<ITelegramBotClient>();
    var updateHandler = services.GetService<IUpdateHandler>();
    botClient.StartReceiving(updateHandler);

    Console.WriteLine($"{Constants.APP_NAME} is all set.");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"Error Occured: {ex}");
}
