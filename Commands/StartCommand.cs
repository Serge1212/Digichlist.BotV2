using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Telegram.Bot;

namespace Digichlist.Bot.Commands
{
    /// <summary>
    /// The Start command that works for <see cref="CommandConstants.START"/> command.
    /// </summary>
    public class StartCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;

        public StartCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var welcomeMessage = "Hi! Welcome to the Digichlist Bot. This bot was created to help you to send defects in a hotel that you found.\n\n" +
            "If you are new to this bot and want to get started, please enter the /registerme command so that our administration will review your request as soon as possible.\n\n" +
            "Press 'Menu' button to see all features.";

            await _botClient.SendTextMessageAsync(message.ChatId, welcomeMessage, cancellationToken: cancellationToken);
        }
    }
}
