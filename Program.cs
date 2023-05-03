using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(cfg => cfg.AddConsole());
        var botToken = Environment.GetEnvironmentVariable("DigichlistV2", EnvironmentVariableTarget.User) ?? throw new ArgumentException("Missing API token for the Telegram Bot.");
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
        // Handlers.
        services.AddTransient<BotHandler>();
        // Commands.
        services.AddTransient<StartCommand>();
        services.AddTransient<RegisterMeCommand>();
        services.AddTransient<NewDefectCommand>();
        services.AddTransient<SetDefectStatusCommand>();
        // Repos.
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IDefectRepository, DefectRepository>();

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
    var updateHandler = services.GetService<BotHandler>();

    if (botClient is null || updateHandler is null)
    {
        throw new InvalidOperationException("Could not configure the bot handlers.");
    }

    // Define receiving options.
    var receiverOps = new ReceiverOptions
    {
        ThrowPendingUpdates = true, // Will ignore all preceding messages.
        AllowedUpdates = AllowedUpdates.GetAllowedUpdates() // Define updates that this bot can handle.
    };

    botClient.StartReceiving(updateHandler, receiverOptions: receiverOps);

    Console.WriteLine($"{Constants.APP_NAME} is all set.");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"Error Occured: {ex}");
}