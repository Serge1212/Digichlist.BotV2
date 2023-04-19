using Digichlist.Bot.Commands;
using Digichlist.Bot.Models;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Digichlist.Bot.Test.Commands
{
    public class StartCommandTests
    {
        readonly Mock<ITelegramBotClient> _botClientMock = new();
        readonly StartCommand _command;

        public StartCommandTests()
        {
            _command = new StartCommand(_botClientMock.Object);
        }

        [Fact]
        public async Task ProcessAsync_Should_RegisterANewUser()
        {
            // Arrange.
            var expectedMessagePart = "want to get started, please enter the";
            var expectedChatId = 13333;
            var message = new BotMessage
            {
                ChatId = expectedChatId,
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldNotProcess_WhenCancellationRequested()
        {
            // Arrange.
            var source = new CancellationTokenSource();
            source.Cancel();

            // Act.
            await _command.ProcessAsync(It.IsAny<BotMessage>(), source.Token);

            // Assert.
            _botClientMock.Verify(b => b.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
