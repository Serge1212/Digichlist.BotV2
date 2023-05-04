namespace Digichlist.Bot.Configuration
{
    /// <summary>
    /// Generalized constants to be used throughout the app.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// The app's name.
        /// </summary>
        public const string APP_NAME = "Digichlist.Bot";

        /// <summary>
        /// The number of buttons to be displayed on a single row in Telegram keyboard.
        /// </summary>
        public const int BUTTONS_ROW_COUNT = 3;

        /// <summary>
        /// The seconds for ongoings tasks after which the ongoing task is considered to be expired.
        /// </summary>
        public const int COMMAND_TASK_EXPIRATION_SECONDS = 60;
    }
}
