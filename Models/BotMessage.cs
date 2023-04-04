using Telegram.Bot.Types;

namespace Digichlist.Bot.Models
{
    /// <summary>
    /// The simplified message information provided from <see cref="Telegram.Bot.Types.Update"/>
    /// </summary>
    public class BotMessage
    {
        public long ChatId { get; set; }

        /// <summary>
        /// Temp prop. Will be removed until all props are defined.
        /// </summary>
        public Message Message { get; set; }


        /// <summary>
        /// Maps <see cref="Update"/> to simplified model.
        /// </summary>
        public static BotMessage ToModel(Update update)
        {
            return new BotMessage
            {
                ChatId = update.Message.Chat.Id,
                Message = update.Message,
            };
        }
    }
}
