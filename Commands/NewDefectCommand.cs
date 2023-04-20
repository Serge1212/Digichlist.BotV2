namespace Digichlist.Bot.Commands
{
    /// <summary>
    /// The NewDefect command that works for <see cref="CommandConstants.NEW_DEFECT"/> command.
    /// Stores the defect provided by a user.
    /// </summary>
    public class NewDefectCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;
        readonly IUserRepository _userRepository;
        readonly IDefectRepository _defectRepository;
        readonly IServiceProvider _services;


        public NewDefectCommand(
            ITelegramBotClient botClient,
            IUserRepository userRepository,
            IDefectRepository defectRepository,
            IServiceProvider services
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _defectRepository = defectRepository;
            _services = services;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;
            var user = await _userRepository.GetByChatIdAsync(chatId);

            //if (user?.Role is Role role && !role.CanAdd)
            //{
            //    await _botClient.SendTextMessageAsync(
            //        chatId,
            //        "Unfortunately your role does not have a permission to publish defects or the role is missing.",
            //        cancellationToken: cancellationToken);
            //}
            var handler = (IUpdateHandler)_services.GetService(typeof(NewDefectCommandHandler));
            
            //TODO: REMAKE BELOW
            // Create delegate handlers here, so we can pass users, defects and ids.
            await _botClient.ReceiveAsync(handler);
        }
    }
}
