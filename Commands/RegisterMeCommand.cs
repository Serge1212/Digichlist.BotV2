using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Telegram.Bot;

namespace Digichlist.Bot.Commands
{
    /// <summary>
    /// The RegisterMe command that works for <see cref="CommandConstants.REGISTER_ME"/> command.
    /// Saves registration request.
    /// </summary>
    public class RegisterMeCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;

        public RegisterMeCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        /// <inheritdoc />
        public async Task Process(BotMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
