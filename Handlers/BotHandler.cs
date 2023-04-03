using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Digichlist.Bot.Handlers
{
    public class BotHandler : IUpdateHandler
    {
        /// <summary>
        /// Entry point of getting Telegram messages.
        /// </summary>
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Exit when cancellation is requested.
            if(cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Temp stub.
            if (update.Message is not null)
            {
                var chatId = update.Message.Chat.Id;
                await botClient.SendTextMessageAsync(chatId, "I know you're here");
            }
        }

        /// <summary>
        /// Handles errors while polling.
        /// </summary>
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}
