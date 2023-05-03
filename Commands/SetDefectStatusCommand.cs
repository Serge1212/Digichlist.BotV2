namespace Digichlist.Bot.Commands
{
    public class SetDefectStatusCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;
        readonly IUserRepository _userRepository;
        readonly IDefectRepository _defectRepository;

        public SetDefectStatusCommand(
            ITelegramBotClient botClient,
            IUserRepository userRepository,
            IDefectRepository defectRepository
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _defectRepository = defectRepository;
        }

        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;
            var user = await _userRepository.GetByChatIdAsync(chatId);

            //if (user?.Role is null || !user.Role.CanBeAssigned)
            //{
            //    await _botClient.SendTextMessageAsync(
            //        chatId,
            //        "Unfortunately your role does not have a permission to work with defects or the role is missing.",
            //        cancellationToken: cancellationToken);
            //    return;
            //}

            var keyboard = GenerateKeyboardAsync(chatId);
            if (keyboard is null)
            {
                await _botClient.SendTextMessageAsync(chatId, "You do not have any assigned defects yet.", cancellationToken: cancellationToken);
                return;
            }
            await _botClient.SendTextMessageAsync(message.ChatId, "please select", replyMarkup: keyboard, cancellationToken: cancellationToken);
        }

        InlineKeyboardMarkup GenerateKeyboardAsync(long chatId)
        {
            // Get all related defects.
            //var defects = _defectRepository.GetDefectsByChatId(chatId);
            var defectsTemp = new List<Defect>
            {
                new Defect
                {
                    Id = 1,
                    RoomNumber = 1,
                    Description = "broken something in room",
                },
                new Defect
                {
                    Id = 2,
                    RoomNumber = 2,
                    Description = "broken something in room",
                },
                new Defect
                {
                    Id = 3,
                    RoomNumber = 3,
                    Description = "broken something in room",
                },
                new Defect
                {
                    Id = 4,
                    RoomNumber = 4,
                    Description = "broken something in room",
                },
                new Defect
                {
                    Id = 5,
                    RoomNumber = 5,
                    Description = "broken something in room",
                },
                new Defect
                {
                    Id = 6,
                    RoomNumber = 6,
                    Description = "broken something in room",
                },
            };
            var defects = defectsTemp.Select(d => d);

            if (!defects.Any())
            {
                return null;
            }

            // Get all keys.
            var keys = defects.Select(d => new InlineKeyboardButton(GetBriefDetails(d)) { CallbackData = GenerateCallback(chatId, d) });

            // Generate a keyboard.
            var keyboard = keys
                .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / Constants.BUTTONS_ROW_COUNT)
                .Select(g => g.Select(x => x.Value));

            return new InlineKeyboardMarkup(keyboard);
        }

        string GetBriefDetails(Defect defect) =>
            string.Concat($"Room {defect.RoomNumber}: {defect.Description[..20]}...");

        static string GenerateCallback(long chatId, Defect defect) =>
           JsonSerializer.Serialize(DefectStatusCallback.ToModel(chatId, defect));
    }
}
