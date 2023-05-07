namespace Digichlist.Bot.Commands
{
    /// <summary>
    /// The NewDefect command that works for <see cref="Configuration.BotCommands.NEW_DEFECT"/> command.
    /// Stores the defect provided by a user.
    /// </summary>
    public class NewDefectCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;
        readonly IUserRepository _userRepository;
        readonly IDefectRepository _defectRepository;

        public NewDefectCommand(
            ITelegramBotClient botClient,
            IUserRepository userRepository,
            IDefectRepository defectRepository
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _defectRepository = defectRepository;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;
            var user = await _userRepository.GetByChatIdAsync(chatId);

            if (user?.Role is null || !user.Role.CanAdd)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Unfortunately your role does not have a permission to publish defects or the role is missing.",
                    cancellationToken: cancellationToken);
                return;
            }

            var incomingMsg = message.Text;
            var defectInfo = incomingMsg.Split(' ');

            if (defectInfo.Length < 3)
            {
                await NotifyDefectInfoInvalidAsync(chatId, cancellationToken);
                return;
            }

            var hasRoomNumber = int.TryParse(defectInfo[1], out int roomNumber);
            var description = string.Join(' ', defectInfo[2..]);


            if (!hasRoomNumber || string.IsNullOrWhiteSpace(description))
            {
                await NotifyDefectInfoInvalidAsync(chatId, cancellationToken);
                return;
            }

            var defect = new Defect
            {
                RoomNumber = roomNumber,
                Description = description,
                CreatedBy = user.GetName(),
                CreatedAt = DateTime.Now,
                Status = (int)DefectStatus.Opened,
            };

            await _defectRepository.AddAsync(defect);

            await _botClient.SendTextMessageAsync(chatId, "The defect was successfully published!", cancellationToken: cancellationToken);
        }

        async Task NotifyDefectInfoInvalidAsync(long chatId, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                    chatId,
                    "Please send a new defect in a following format: /newdefect [room number] [description]",
                    cancellationToken: cancellationToken);
        }
    }
}
