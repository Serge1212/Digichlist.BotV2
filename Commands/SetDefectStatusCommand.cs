namespace Digichlist.Bot.Commands
{
    public class SetDefectStatusCommand : IBotCommand
    {
        readonly ITelegramBotClient _botClient;
        readonly IUserRepository _userRepository;
        readonly IDefectRepository _defectRepository;
        readonly ICommandTaskRepository _commandTaskRepository;
        readonly ILogger _logger;

        public SetDefectStatusCommand(
            ITelegramBotClient botClient,
            IUserRepository userRepository,
            IDefectRepository defectRepository,
            ICommandTaskRepository commandTaskRepository,
            ILogger<SetDefectStatusCommand> logger
            )
        {
            _botClient = botClient;
            _userRepository = userRepository;
            _defectRepository = defectRepository;
            _commandTaskRepository = commandTaskRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;

            // Role check is performed for each subflow.
            if (!await CheckUserRole(chatId, cancellationToken))
            {
                return;
            }

            // Define and execute command subflow.
            if (message.Text is not null) // The user initialized setting status.
            {
                await ProcessInitializedCommandAsync(message, cancellationToken);
            }
            else if (message.Data is string rawData) // The task is ongoing, the user inputs more data (e.g defect or status info).
            {
                await ProcessOngoingCommandAsync(message, rawData, cancellationToken);
            }
        }

        /// <summary>
        /// Tries to settle the ongoing command tasks for this user.
        /// </summary>
        /// <returns>
        /// <c>true</c> when no ongoing tasks for this user.
        /// <c>false</c> when there are ongoing tasks for this user that cannot be settled.
        /// </returns>
        async Task<bool> TrySettleOngoingTaskAsync(long chatId, CancellationToken cancellationToken)
        {
            var commandTask = await _commandTaskRepository.GetAsync(chatId);

            if (commandTask is null)
            {
                return true;
            }

            var now = DateTime.Now;
            if (commandTask.ExpiresAt < now)
            {
                commandTask.ClosedAt = now;
                await _commandTaskRepository.UpdateAsync(commandTask);
                _logger.LogDebug("Found and settled expired ongoing task for chat #{chatId}. Ongoing task info: {@commandTask}", chatId, commandTask);

                return true;
            }

            await _botClient.SendTextMessageAsync(
                    chatId,
                    "You have uncompleted commands that cannot be continued. Please enter /cancel to close them. Then you can start fresh.",
                    cancellationToken: cancellationToken);
            return false;
        }

        async Task ProcessInitializedCommandAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;

            // Try settle previous command task.
            if (!await TrySettleOngoingTaskAsync(chatId, cancellationToken))
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
            await _commandTaskRepository.AddAsync(ongoingTask);

            // Send keyboard.
            await SendDefectKeyboardAsync(message, cancellationToken);
        }

        async Task ProcessOngoingCommandAsync(BotMessage message, string rawData, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;
            var commandTask = await _commandTaskRepository.GetAsync(chatId);
            var data = JsonSerializer.Deserialize<CommandCallback>(rawData);

            if (commandTask is null)
            {
                throw new InvalidOperationException("Cannot process further without related command task.");
            }

            //if (data?.TaskId != commandTask.Id)
            //{
            //    throw new InvalidOperationException("The found command task is not the same for callback's identifier.");
            //}

            if (data is null)
            {
                _logger.LogError("Cannot deserialize incoming callback data: {rawData}. The command cannot be processed further.", rawData);
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Something went wrong. Please contact administrators.",
                    cancellationToken: cancellationToken);

                return;
            }

            if (!await ValidateIncomingDefectAsync(chatId, data.DefectId, cancellationToken))
            {
                return;
            }

            if (data.Status == null) // The user picked the defect.
            {
                // Drop the message with defects.
                await _botClient.DeleteMessageAsync(chatId, message.MessageId.Value, cancellationToken: cancellationToken);

                // Send keyboard with statuses.
                commandTask.DefectId = data.DefectId;
                await _commandTaskRepository.UpdateAsync(commandTask);
                await SendStatusKeyboardAsync(chatId, data.DefectId, cancellationToken);
            }
            else if (data.Status != null) // The user picked the status.
            {
                if (data.DefectId < 1 || !data.Status.HasValue)
                {
                    _logger.LogError("Expected status info on this step, but incoming info was: {rawData}. The command cannot be processed further.", rawData);
                    await _botClient.SendTextMessageAsync(
                    chatId,
                    "Something went wrong. Please contact administrators.",
                    cancellationToken: cancellationToken);
                    
                    return;
                }

                // Complete command task.
                commandTask.Status = (int)data.Status.Value;
                commandTask.ClosedAt = DateTime.Now;
                await _commandTaskRepository.UpdateAsync(commandTask);

                // Set new status.
                var defect = await _defectRepository.GetSingleAsync(data.DefectId);
                defect.Status = (int)data.Status;
                await _defectRepository.UpdateAsync(defect);

                // Notify user on status changed.
                await _botClient.EditMessageTextAsync(chatId, message.MessageId.Value, "The status was successfully changed!", cancellationToken: cancellationToken);

            }
            else
            {
                _logger.LogWarning("Nothing was processed for message {@message}", message);
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

        async Task SendDefectKeyboardAsync(BotMessage message, CancellationToken cancellationToken)
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

        async Task SendStatusKeyboardAsync(long chatId, int defectId, CancellationToken cancellationToken)
        {
            // Grab all statuses.
            var statuses = new List<(string Name, int Signifier)>
            {
                ("Opened", (int)DefectStatus.Opened),
                ("Fixing",(int)DefectStatus.Fixing),
                ("Eliminated", (int)DefectStatus.Eliminated),
            };

            // Get all keys.
            var keys = statuses.Select(status => new InlineKeyboardButton(status.Name) { CallbackData = JsonSerializer.Serialize(new { Command = BotCommands.SET_DEFECT_STATUS, Status = status.Signifier, DefectId = defectId}) });

            // Generate keyboard.
            var keyboard = new InlineKeyboardMarkup(keys);

            // Send keyboard.
            await _botClient.SendTextMessageAsync(chatId, "Please select the status.", replyMarkup: keyboard, cancellationToken: cancellationToken);
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
            var defects = _defectRepository.GetManyByChatId(chatId);

            if (!defects.Any())
            {
                return null;
            }

            // Get all keys.
            var keys = defects.Select(d => new InlineKeyboardButton(GetBriefDetails(d)) { CallbackData = JsonSerializer.Serialize(new { Command = BotCommands.SET_DEFECT_STATUS, DefectId = d.Id }) });

            // Generate a keyboard.
            var keyboard = keys
                .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / Constants.BUTTONS_ROW_COUNT)
                .Select(g => g.Select(x => x.Value));

            return new InlineKeyboardMarkup(keyboard);
        }

        string GetBriefDetails(Defect defect)
        {
            if (defect.Description.Length > 20)
            {
                defect.Description = $"{defect.Description[..20]}...";
            }

            return string.Concat($"Room {defect.RoomNumber}: {defect.Description}");
        }
    }
}
