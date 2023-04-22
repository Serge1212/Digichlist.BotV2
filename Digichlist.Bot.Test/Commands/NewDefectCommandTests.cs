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

        [Theory]
        [InlineData("/newdefect 12 description", 12, "description")]
        [InlineData("/newdefect 25 description 12 43", 25, "description 12 43")]
        [InlineData("/newdefect 1 2 description", 1, "2 description")]
        [InlineData("/newdefect 1 very big description for test puroses", 1, "very big description for test puroses")]
        public async Task ProcessAsync_ShouldPublishNewDefect(string msg, int roomNumberPart, string descriptionPart)
        {
            // Arrange.
            var expectedChatId = 13333;
            var expectedFirstName = "test";

            _userRepositoryMock
                .Setup(u => u.GetByChatIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new User
                {
                    ChatId = expectedChatId,
                    FirstName = expectedFirstName,
                    Role = new Role
                    {
                        CanAdd = true,
                    }
                });

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {
                    Text = msg
                },
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _defectRepositoryMock.Verify(d => d.SaveAsync(It.Is<Defect>(d =>
            d.RoomNumber == roomNumberPart &&
            d.Description == descriptionPart &&
            d.CreatedBy!.Contains(expectedFirstName)
            )));
        }

        [Theory]
        [InlineData("/newdefect twelve test description")]
        [InlineData("          ")]
        [InlineData("  ")]
        [InlineData("/newdefect 12description")]
        [InlineData("/newdefect 12")]
        public async Task ProcessAsync_ShouldNotPublish_NewDefect_WithIncorrectInfo(string msg)
        {
            // Arrange.
            var expectedChatId = 13333;
            var expectedMessagePart = "send a new defect in a following format";

            _userRepositoryMock
                .Setup(u => u.GetByChatIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new User
                {
                    ChatId = expectedChatId,
                    FirstName = "test",
                    Role = new Role
                    {
                        CanAdd = true,
                    }
                });

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {
                    Text = msg
                },
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _defectRepositoryMock.Verify(d => d.SaveAsync(It.IsAny<Defect>()), Times.Never);
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldNotPublish_NewDefect_ForUnprivilegedUser()
        {
            // Arrange.
            var expectedChatId = 13333;
            var expectedMessagePart = "your role does not have a permission";

            _userRepositoryMock
                .Setup(u => u.GetByChatIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new User
                {
                    ChatId = expectedChatId,
                    FirstName = "test",
                    Role = new Role
                    {
                        CanAdd = false,
                    }
                });

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/newdefect 12 description",
                },
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _defectRepositoryMock.Verify(d => d.SaveAsync(It.IsAny<Defect>()), Times.Never);
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldNotPublish_NewDefect_ForUserWithNoRole()
        {
            // Arrange.
            var expectedChatId = 13333;
            var expectedMessagePart = "your role does not have a permission";

            _userRepositoryMock
                .Setup(u => u.GetByChatIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new User
                {
                    ChatId = expectedChatId,
                    FirstName = "test",
                    Role = null,
                });

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/newdefect 12 description",
                },
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _defectRepositoryMock.Verify(d => d.SaveAsync(It.IsAny<Defect>()), Times.Never);
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldNotPublish_NewDefect_ForUser_ThatDoesNotExist()
        {
            // Arrange.
            var expectedChatId = 13333;
            var expectedMessagePart = "your role does not have a permission";

            _userRepositoryMock
                .Setup(u => u.GetByChatIdAsync(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            var message = new BotMessage
            {
                ChatId = expectedChatId,
                Message = new Telegram.Bot.Types.Message
                {
                    Text = "/newdefect 12 description",
                },
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _defectRepositoryMock.Verify(d => d.SaveAsync(It.IsAny<Defect>()), Times.Never);
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == expectedChatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }
    }
}
