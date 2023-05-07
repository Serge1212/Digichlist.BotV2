namespace Digichlist.Bot.Handlers
{
    /// <summary>
    /// The main class that handles all incoming messages from bot.
    /// </summary>
    public class BotHandler : IUpdateHandler
    {
        readonly IServiceProvider _services;
        readonly ILogger _logger;

        public BotHandler(
            IServiceProvider services,
            ILogger<BotHandler> logger
            )
        {
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// Handles errors while polling.
        /// </summary>
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Entry point of getting Telegram messages.
        /// </summary>
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Gracefully exit when cancellation is requested.
            if(cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Exit when the incoming information is invalid/missing.
            if(!await ValidateMessageAsync(botClient, update))
            {
                _logger.LogError("Update was invalid. Info: {@Update}", update);
                return;
            }

            var chatId = update?.Message?.Chat?.Id ?? update?.CallbackQuery?.From?.Id;
            var commandText = update?.Message?.Text;

            if (commandText is null) // it's a callback query then.
            {
                var data = JsonSerializer.Deserialize<CommandCallback>(update?.CallbackQuery?.Data);
                commandText = data?.Command;

                if (commandText == null)
                {
                    _logger.LogWarning("Could not get a command from user input. The processing will not happen.");
                    return;
                }
            }

            try
            {
                // Grab all needed info for further procesing.
                var message = BotMessage.ToModel(update);

                // Find suitable command.
                var command = GetCommand(commandText);

                // Process command.
                await command.ProcessAsync(message, cancellationToken);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError("Error while command execution: {message}", ex.Message);
                await botClient.SendTextMessageAsync(chatId, $"There is no such available command - {commandText}. Please take a look at the Menu.", cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        async Task<bool> ValidateMessageAsync(ITelegramBotClient botClient, Update update)
        {
            if (update?.Message is Message message)
            {
                return await ValidateTextMessageAsync(botClient, message);
            }
            else if (update?.CallbackQuery is CallbackQuery query)
            {
                return await ValidateCallbackQueryAsync(botClient, query);
            }
            return false; // skip any other actions.
        }

        private static Task<bool> ValidateCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery query)
        {
            if (query.Data is null || query.From is null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        async Task<bool> ValidateTextMessageAsync(ITelegramBotClient botClient, Message message)
        {
            var messageInfo = JsonSerializer.Serialize(message);
            // High-level validation.
            if (
                message.Chat is null || // no chat info.
                message.Chat.Id < 0 // no chat identifier.
                )
            {
                _logger.LogError("Some of the information is missing: {messageInfo}", messageInfo);
                return false;
            }

            // Low-level validation.
            var text = message.Text;
            if (
                string.IsNullOrWhiteSpace(text) || // no command was passed.
                (text.Split(' ').Length > 1 && !text.Contains(Configuration.BotCommands.NEW_DEFECT)) // the command format is definitely not correct.
                )
            {
                _logger.LogError("The command message was incorrect: {messageInfo}", messageInfo);
                var chatId = message.Chat.Id;
                await botClient.SendTextMessageAsync(chatId, "Please send a valid command. You may find them in the Menu");
                return false;
            }

            return true;
        }

        IBotCommand GetCommand(string? command) => command switch
        {
            // Start command.
            Configuration.BotCommands.START => ResolveCommand<StartCommand>(),
            // RegisterMe command.
            Configuration.BotCommands.REGISTER_ME => ResolveCommand<RegisterMeCommand>(),
            // NewDefect command.
            var c when c.Contains(Configuration.BotCommands.NEW_DEFECT) => ResolveCommand<NewDefectCommand>(),
            // Cancel command.
            Configuration.BotCommands.CANCEL => ResolveCommand<CancelCommand>(),
            // SetDefectStatus command.
            Configuration.BotCommands.SET_DEFECT_STATUS => ResolveCommand<SetDefectStatusCommand>(),
            _ => throw new ArgumentOutOfRangeException(nameof(command)),

        };

        IBotCommand ResolveCommand<T>() where T : IBotCommand
        {
            return (IBotCommand)_services.GetService(typeof(T));
        }
    }
}
