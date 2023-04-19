using Digichlist.Bot.Commands;
using Digichlist.Bot.Configuration;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

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
            if(!(await ValidateMessageAsync(botClient, update)))
            {
                return;
            }

            var chatId = update!.Message!.Chat.Id;
            var commandText = update!.Message!.Text;

            try
            {
                var message = BotMessage.ToModel(update);
                var command = GetCommand(commandText);
                await command.ProcessAsync(message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError("Error while command execution: {message}", ex.Message);
                await botClient.SendTextMessageAsync(chatId, $"There is no such available command - {commandText}. Please take a look at the menu.", cancellationToken: cancellationToken);
            }
        }

        async Task<bool> ValidateMessageAsync(ITelegramBotClient botClient, Update update)
        {
            var messageInfo = JsonSerializer.Serialize(update?.Message);
            // High-level validation.
            if (
                update is null || // no info came at all.
                update.Message is null || // no message info.
                update.Message.Chat is null || // no chat info.
                update.Message.Chat.Id < 0 // no chat identifier.
                )
            {
                _logger.LogError("Some of the information is missing: {messageInfo}", messageInfo);
                return false;
            }

            // Low-level validation.
            if (
                string.IsNullOrWhiteSpace(update.Message.Text) || // no command was passed.
                update.Message.Text.Split(' ').Length > 1 // the command format is definitely not correct.
                )
            {
                _logger.LogError("The command message was incorrect: {messageInfo}", messageInfo);
                var chatId = update.Message.Chat.Id;
                await botClient.SendTextMessageAsync(chatId, "Please send a valid command. You may find them in the menu");
                return false;
            }

            return true;
        }

        IBotCommand GetCommand(string? command) => command switch
        {
            CommandConstants.START => ResolveCommand<StartCommand>(),
            CommandConstants.REGISTER_ME => ResolveCommand<RegisterMeCommand>(),
            _ => throw new ArgumentOutOfRangeException(nameof(command)),
        };

        IBotCommand ResolveCommand<T>() where T : IBotCommand
        {
            return (IBotCommand)_services.GetService(typeof(T));
        }
    }
}
