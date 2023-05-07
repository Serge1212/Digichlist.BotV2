namespace Digichlist.Bot.Configuration
{
    /// <summary>
    /// The class that contains information about all available bot commands.
    /// </summary>
    public class BotCommands
    {
        /// <summary>
        /// The "start" command. Shows essential information about bot.
        /// </summary>
        public const string START = "/start";

        /// <summary>
        /// The "registerme" command. Sends request from a sender for them to be registered.
        /// </summary>
        public const string REGISTER_ME = "/registerme";

        /// <summary>
        /// The "newdefect" command. Sends information about a defect.
        /// </summary>
        public const string NEW_DEFECT = "/newdefect";

        /// <summary>
        /// The "setdefectstatus" command. Used to set a defect status.
        /// </summary>
        public const string SET_DEFECT_STATUS = "/setdefectstatus";

        /// <summary>
        /// TODO: explain
        /// </summary>
        public const string CANCEL = "/cancel";
    }
}
