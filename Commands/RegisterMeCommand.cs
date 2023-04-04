using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Digichlist.Bot.Models.Entities;
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
            var chatId = message.ChatId;

            var user = new User(); // TODO: Temp Stub.

            //TODO: This could be moved to another place once privileges for users are done for the Telegram bot.
            if (user is null)
            {
                await AddUserAsync(message); // Save user to the database. This is the only place where a user can be saved to the database.
                await _botClient.SendTextMessageAsync(chatId, "The registration request was successfully sent!\n You'll be notified as soon as possible!");
            }
            else if (!user.IsRegistered)
            {
                await _botClient.SendTextMessageAsync(chatId, "You've already requested the registration. Our admins are working on it.");
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "You are already registered");
            }
        }

        Task AddUserAsync(BotMessage message)
        {
            var user = new User
            {
                FirstName = message.Message?.From?.FirstName ?? "N/A",
                LastName = message.Message?.From?.LastName ?? "N/A",
                Username = message.Message?.From?.Username ?? "N/A",
                ChatId = message.ChatId
            };

            //TODO: Save user to a database;
            return Task.CompletedTask;
        }
    }
}
