using Digichlist.Bot.Commands;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Digichlist.Bot.Models.Entities;
using Moq;
using System.Threading;
using Telegram.Bot;
using Xunit;

namespace Digichlist.Bot.Test.Commands
{
    public class NewDefectCommandTests
    {
        readonly Mock<ITelegramBotClient> _botClientMock = new();
        readonly Mock<IUserRepository> _userRepositoryMock = new();
        readonly Mock<IDefectRepository> _defectRepositoryMock = new();
        readonly NewDefectCommand _command;

        public NewDefectCommandTests()
        {
            _command = new NewDefectCommand(_botClientMock.Object, _userRepositoryMock.Object, _defectRepositoryMock.Object);
        }

        [Fact]
        public async void ProcessAsync_ShouldPublishNewDefect()
        {
            // Arrange.
            var expectedChatId = 13333;

            var user = new User
            {
                ChatId = expectedChatId,
                Id = 1,
                Role = new Role
                {
                    Id = 1,
                    Name = "TestRole",
                    CanAdd = true,
                    CanBeAssigned = false,
                }
            };

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {

                }
            };

            // Act.
            await _command.ProcessAsync(null, CancellationToken.None);

            // Assert.
        }
    }
}
