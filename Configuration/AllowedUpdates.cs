using Telegram.Bot.Types.Enums;

namespace Digichlist.Bot.Configuration
{
    /// <summary>
    /// The class that defines the allowed updates that this bot can handle.
    /// </summary>
    public class AllowedUpdates
    {
        /// <summary>
        /// Returns allowed update actions for this bot.
        /// </summary>
        public static UpdateType[] GetAllowedUpdates()
        {
            return new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery };
        }
    }
}
