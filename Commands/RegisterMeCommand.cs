using User = Digichlist.Bot.Models.Entities.User;

namespace Digichlist.Bot.Commands
{
    /// <summary>
    /// The RegisterMe command that works for <see cref="CommandConstants.REGISTER_ME"/> command.
    /// Saves registration request.
    /// </summary>
    public class RegisterMeCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;
        readonly IUserRepository _userRepository;

        public RegisterMeCommand(ITelegramBotClient botClient, IUserRepository userRepository)
        {
            _botClient = botClient;
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var chatId = message.ChatId;

            var user = await _userRepository.GetByChatIdAsync(chatId);

            if (user is null)
            {
                // Save user to the database.
                // This is the only place where a user can be saved to the database.
                await AddUserAsync(message);
                await _botClient.SendTextMessageAsync(chatId, "The registration request was successfully sent!\n You'll be notified as soon as possible!", cancellationToken: cancellationToken);
            }
            else if (user.IsRegistered)
            {
                await _botClient.SendTextMessageAsync(chatId, "You are already registered", cancellationToken: cancellationToken);
            }
            else
            {
                var msg = await _botClient.SendTextMessageAsync(chatId, "You've already requested the registration. Our admins are working on it.", cancellationToken: cancellationToken);
            }
        }

        async Task AddUserAsync(BotMessage message)
        {
            var user = new User
            {
                FirstName = message.Message?.From?.FirstName ?? "N/A",
                LastName = message.Message?.From?.LastName ?? "N/A",
                Username = message.Message?.From?.Username ?? "N/A",
                ChatId = message.ChatId
            };

            await _userRepository.SaveUserAsync(user);
        }
    }
}
