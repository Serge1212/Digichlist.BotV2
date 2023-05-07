namespace Digichlist.Bot.Models
{
    /// <summary>
    /// The simplified message information provided from <see cref="Telegram.Bot.Types.Update"/>
    /// </summary>
    public class BotMessage
    {
        /// <summary>
        /// The user's unique chat identifier.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// TODO: Explain.
        /// </summary>
        public int? MessageId { get; set; }

        /// <summary>
        /// The specified passed data.
        /// Null means that there is no data hence it's not a <see cref="Update.CallbackQuery"/>.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// The specified plain text.
        /// Null means that there is no text hence it's not a <see cref="Update.Message"/>.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        public string? Username { get; set; }


        /// <summary>
        /// Maps <see cref="Update"/> to simplified model.
        /// </summary>
        public static BotMessage ToModel(Update update)
        {
            var userInfo = update.Message != null
                ? update.Message.From
                : update?.CallbackQuery?.From;

            if (userInfo is null)
            {
                throw new InvalidOperationException("Cannot map user info for further processing.");
            }

            if (update?.CallbackQuery?.Data is null && update?.Message?.Text is null)
            {
                throw new InvalidOperationException("No useful incoming info was passed for further processing.");
            }

            return new BotMessage
            {
                ChatId = userInfo.Id,
                MessageId = update?.CallbackQuery?.Message?.MessageId,
                Data = update?.CallbackQuery?.Data,
                Text = update?.Message?.Text,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Username = userInfo.Username,
            };
        }
    }
}
