namespace Digichlist.Bot.Commands
{
    /// <summary>
    /// The command for cancelling the ongiing <see cref="CommandTask"/>s for specified chat identifier.
    /// </summary>
    public class CancelCommand : IBotCommand
    {
        readonly ICommandTaskRepository _commandTaskRepository;
        readonly ITelegramBotClient _botClient;

        public CancelCommand(ICommandTaskRepository commandTaskRepository, ITelegramBotClient botClient)
        {
            _commandTaskRepository = commandTaskRepository;
            _botClient = botClient;
        }


        public async Task ProcessAsync(BotMessage message, CancellationToken cancellationToken)
        {
            var chatId = message.ChatId;
            var commandTask = await _commandTaskRepository.GetAsync(chatId);

            if (commandTask is null)
            {
                await _botClient.SendTextMessageAsync(chatId, "You have no ongoing commands to cancel.");
                return;
            }

            commandTask.ClosedAt = DateTime.Now;
            await _commandTaskRepository.UpdateAsync(commandTask);

            await _botClient.SendTextMessageAsync(chatId, "All ongoing commands were canceled.");
        }
    }
}
