using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Digichlist.Bot.Models.Entities;
using Telegram.Bot;

namespace Digichlist.Bot.Commands
{
    public class NewDefectCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;

        public NewDefectCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task Process(BotMessage message)
        {
            //if (!(await ValidateUserAsync(message.ChatId)) {
            //
            //}
        }

        private async Task<bool> ValidateUserAsync(long chatId)
        {
            var user = new User(); //TODO: Remove temp stub.

            if (user is null)
            {
                await _botClient.SendTextMessageAsync(chatId, "You cannot publish defects as you are not registered.\n\n Press or type /registerme command to request the registration.\n Our admins will respond as soon as possible.");
                return false;
            }

            if (user?.Role?.CanAdd == true)
            {
                await _botClient.SendTextMessageAsync(chatId, "Unfortunately you have no permission to publish defects.");
                return false;
            }

            return true;
        }

        //private async Task 
    }
}
