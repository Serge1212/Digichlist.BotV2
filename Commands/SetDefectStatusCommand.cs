namespace Digichlist.Bot.Commands
{
    public class SetDefectStatusCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;
        readonly IUserRepository _userRepository;
        readonly IDefectRepository _defectRepository;
        readonly ICommandTaskRepository _commandTaskInfoRepository;
        readonly ILogger _logger;

        public SetDefectStatusCommand(
            ITelegramBotClient botClient,
            IUserRepository userRepository,
            IDefectRepository defectRepository,
            ICommandTaskRepository commandTaskInfoRepository,
            ILogger<SetDefectStatusCommand> logger
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _defectRepository = defectRepository;
            _commandTaskInfoRepository = commandTaskInfoRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;

            // Try settle previous command task.
            if (!await TrySettleOngoingTaskAsync(chatId))
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "You have uncompleted commands that cannot be continued. Please enter /cancel to close them. Then you can start fresh.",
                    cancellationToken: cancellationToken);
                return;
            }

            if (!await CheckUserRole(chatId, cancellationToken))
            {
                return;
            }

            // Create new ongoing command task.
            var ongoingTask = new CommandTask
            {
                ChatId = chatId,
                CommandName = BotCommands.SET_DEFECT_STATUS,
                ExpiresAt = DateTime.Now.AddSeconds(Constants.COMMAND_TASK_EXPIRATION_SECONDS)
            };
            await _commandTaskInfoRepository.AddAsync(ongoingTask);

            // Define and execute command subflow.
            if (message.Text is not null) // The user initialized setting status.
            {
                await SendDefectKeyboardAsync(message);
            }
            else if (message.Data is string rawData)
            {
                var data = JsonSerializer.Deserialize<DefectStatusCallback>(rawData);
                if (data is null)
                {
                    _logger.LogError("Cannot deserialize incoming callback data: {rawData}. The command cannot be processed further.", rawData);
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "Something went wrong. Please contact administrators.",
                        cancellationToken: cancellationToken);

                    return;
                }

                if (data.Status == null && await ValidateIncomingDefectAsync(chatId, data.DefectId, cancellationToken)) // The user picked the defect.
                {
                    await SendStatusKeyboardAsync();
                }
            }
        }

        async Task<bool> CheckUserRole(long chatId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByChatIdAsync(chatId);
            if (user?.Role is null || !user.Role.CanBeAssigned)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Unfortunately your role does not have a permission to work with defects or the role is missing.",
                    cancellationToken: cancellationToken);

                return false;
            }

            return true;
        }

        async Task SendDefectKeyboardAsync(BotMessage message)
        {
            var chatId = message.ChatId;
            // Generate a keyboard with defects.
            var keyboard = GenerateKeyboardAsync(chatId);
            if (keyboard is null)
            {
                await _botClient.SendTextMessageAsync(chatId, "You do not have any assigned defects yet.", cancellationToken: cancellationToken);
                return;
            }

            // Send keyboard.
            await _botClient.SendTextMessageAsync(message.ChatId, "Please select the defect to change its status.", replyMarkup: keyboard, cancellationToken: cancellationToken);
        }

        async Task<bool> ValidateIncomingDefectAsync(long chatId, int defectId, CancellationToken cancellationToken)
        {
            var defect = await _defectRepository.GetSingleAsync(defectId);

            if (defect is null || defect.ClosedAt.HasValue)
            {
                await _botClient.SendTextMessageAsync(
                        chatId,
                        "The defect is already closed",
                        cancellationToken: cancellationToken);
                return false;
            }
            else if (defect.AssignedWorker?.ChatId != chatId)
            {
                await _botClient.SendTextMessageAsync(
                        chatId,
                        "This defect is no longer assigned to you",
                        cancellationToken: cancellationToken);
                return false;
            }

            return true;
        }

        InlineKeyboardMarkup GenerateKeyboardAsync(long chatId)
        {
            // Get all related defects.
            //var defects = _defectRepository.GetDefectsByChatId(chatId);
            //TODO: remove temp stub when not needed.
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
           JsonSerializer.Serialize(DefectStatusCallback.ToModel(defect.Id, BotCommands.SET_DEFECT_STATUS));

        /// <summary>
        /// Tries to settle the ongoing command tasks for this user.
        /// </summary>
        /// <returns>
        /// <c>true</c> when no ongoing tasks for this user.
        /// <c>false</c> when there are ongoing tasks for this user that cannot be settled.
        /// </returns>
        async Task<bool> TrySettleOngoingTaskAsync(long chatId)
        {
            var commandTask = await _commandTaskInfoRepository.GetAsync(chatId);

            if (commandTask is null)
            {
                return true;
            }

            var now = DateTime.Now;
            if (commandTask.ExpiresAt < now)
            {
                commandTask.ClosedAt = now;
                await _commandTaskInfoRepository.UpdateAsync(commandTask);
                _logger.LogDebug("Found and settled expired ongoing task for chat #{chatId}. Ongoing task info: {@commandTask}", chatId, commandTask);

                return true;
            }

            return false;
        }
    }
}
