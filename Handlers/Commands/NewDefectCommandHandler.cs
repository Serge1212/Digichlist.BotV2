namespace Digichlist.Bot.Handlers.Commands
{
    // TODO: add summary if needed.
    public class NewDefectCommandHandler : IUpdateHandler
    {
        readonly Dictionary<long, short> _conversationState = new();
        readonly IDefectRepository _defectRepository;

        public NewDefectCommandHandler(IDefectRepository defectRepository)
        {
            _defectRepository = defectRepository;
        }

        /// <summary>
        /// Handles errors while polling.
        /// </summary>
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        // TODO: add summary if needed.
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update?.Message is null)
            {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var text = update.Message.Text;

            if (!_conversationState.ContainsKey(chatId))
            {
                // TODO: explain if needed.
                _conversationState[chatId] = 1;
            }

            await botClient.SendTextMessageAsync(chatId, "You are at adding defects");
        }

        async Task DoStepAsync(short step)
        {
            switch (step)
            {
                case 1:
                    Console.WriteLine(1);
                    break;
                case 2:
                    Console.WriteLine(2);
                    break;
                case 3:
                    Console.WriteLine(3);
                    break;
            }
        }


    }
}
