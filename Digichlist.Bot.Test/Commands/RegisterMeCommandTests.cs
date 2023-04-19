using Digichlist.Bot.Commands;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Digichlist.Bot.Models.Entities;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;

namespace Digichlist.Bot.Test.Commands
{
    public class RegisterMeCommandTests
    {
        readonly Mock<IUserRepository> _userRepositoryMock = new();
        readonly Mock<ITelegramBotClient> _botClientMock = new();
        readonly RegisterMeCommand _command;

        public RegisterMeCommandTests()
        {
            _command = new RegisterMeCommand(_botClientMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task ProcessAsync_Should_RegisterANewUser()
        {
            // Arrange.
            var expectedMessagePart = "was successfully sent";
            var expectedChatId = 123123;
            var expectedId = 0;
            var expectedFirstName = "Test";
            var expectedLastName = "User";
            var expectedUsername = "username";

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {
                    From = new Telegram.Bot.Types.User()
                }
            };

            _userRepositoryMock
                .Setup(u => u.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _userRepositoryMock.Verify(u => u.GetByIdAsync(It.Is<long>(i => i == expectedChatId)));
            _userRepositoryMock.Verify(u => u.SaveUserAsync(It.IsAny<User>()));
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldDeny_RegisteringExistingUser()
        {
            // Arrange.
            var expectedMessagePart = "already requested the registration";
            var expectedChatId = 123123;

            var message = new BotMessage
            {
                ChatId = expectedChatId,
            };

            _userRepositoryMock
                .Setup(u => u.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(() => new User { IsRegistered = false }); // User exists in db, but registration is not confirmed.

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _userRepositoryMock.Verify(u => u.SaveUserAsync(It.IsAny<User>()), Times.Never);
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldDenyRegistering_RegisteredUser()
        {
            // Arrange.
            var expectedMessagePart = "already registered";
            var expectedChatId = 123123;

            var message = new BotMessage
            {
                ChatId = expectedChatId,
            };

            _userRepositoryMock
                .Setup(u => u.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(() => new User { IsRegistered = true });

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _userRepositoryMock.Verify(u => u.SaveUserAsync(It.IsAny<User>()), Times.Never);
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
            _userRepositoryMock.Verify(u => u.GetByIdAsync(It.IsAny<long>()), Times.Never);
            _userRepositoryMock.Verify(u => u.SaveUserAsync(It.IsAny<User>()), Times.Never);
            _botClientMock.Verify(b => b.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
