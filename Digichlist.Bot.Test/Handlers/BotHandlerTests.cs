using Digichlist.Bot.Commands;
using Digichlist.Bot.Configuration;
using Digichlist.Bot.Handlers;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace Digichlist.Bot.Test.Handlers
{
    public class BotHandlerTests
    {
        readonly Mock<IServiceProvider> _serviceProviderMock = new();
        readonly Mock<ILogger<BotHandler>> _loggerMock = new();
        readonly Mock<ITelegramBotClient> _botClient = new();
        readonly BotHandler _handler;

        public BotHandlerTests()
        {
            _handler = new BotHandler(_serviceProviderMock.Object, _loggerMock.Object);

            var dummyCommandHandler = new Mock<IBotCommand>();
            dummyCommandHandler
                .Setup(c => c.ProcessAsync(It.IsAny<BotMessage>(), CancellationToken.None));

            _serviceProviderMock
                .Setup(provider => provider.GetService(It.IsAny<Type>()))
                .Returns(dummyCommandHandler.Object);
        }

        [Theory]
        [InlineData(CommandConstants.START, typeof(StartCommand))]
        [InlineData(CommandConstants.REGISTER_ME, typeof(RegisterMeCommand))]
        public async Task HandleUpdateAsync_ShouldProcess_AnyCommand(string command, Type commandType)
        {
            // Arrange.
            var update = GetGoodUpdate(It.IsAny<long>(), command);

            // Act.
            await _handler.HandleUpdateAsync(_botClient.Object, update, CancellationToken.None);

            // Assert.
            _serviceProviderMock.Verify(s => s.GetService(It.Is<Type>(service => service == commandType)));
        }


        [Theory]
        [MemberData(nameof(InvalidUpdateModel))]
        public async Task HandleUpdateAsync_ShouldNotProcessCommand_MissingUpdate(Update update)
        {
            // Arrange.
            var expectedMessagePart = "information is missing";

            // Act.
            await _handler.HandleUpdateAsync(_botClient.Object, update, CancellationToken.None);

            // Assert.
            VerifyLoggerMessage(expectedMessagePart);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("        ")]
        [InlineData("")]
        [InlineData("/start right now")]
        [InlineData("  start right now  ")]
        public async Task HandleUpdateAsync_ShouldNotProcessCommand_OnInvalidCommandText(string command)
        {
            // Arrange.
            var expectedMessagePart = "The command message was incorrect:";
            var update = GetGoodUpdate(It.IsAny<long>(), command);

            // Act.
            await _handler.HandleUpdateAsync(_botClient.Object, update, CancellationToken.None);

            // Assert.
            VerifyLoggerMessage(expectedMessagePart);
        }

        [Theory]
        [InlineData("/startrightnow")]
        [InlineData("start")]
        [InlineData("//registerme")]
        public async Task HandleUpdateAsync_ShouldNotProcess_OnInvalidCommand(string command)
        {
            // Arrange.
            var expectedMessagePart = "Error while command execution";
            var update = GetGoodUpdate(It.IsAny<long>(), command);

            // Act.
            await _handler.HandleUpdateAsync(_botClient.Object, update, CancellationToken.None);

            // Assert.
            VerifyLoggerMessage(expectedMessagePart);
        }

        [Fact]
        public async Task HandleUpdateAsync_ShouldNotProcess_WhenCancellationRequested()
        {
            // Arrange.
            var update = GetGoodUpdate(It.IsAny<long>(), "/start");
            var source = new CancellationTokenSource();
            source.Cancel();

            // Act.
            await _handler.HandleUpdateAsync(_botClient.Object, update, source.Token);

            // Assert.
            _serviceProviderMock.Verify(s => s.GetService(It.IsAny<Type>()), Times.Never);
        }

        #region Theory Data
        public static IEnumerable<object[]> InvalidUpdateModel()
        {
            yield return new object[] { null };
            yield return new object[] { new Update { Message = null } };
            yield return new object[] { new Update { Message = new Message { Chat = null } } };
            yield return new object[] { new Update { Message = new Message { Chat = new Chat { Id = -9 } } } };
        }
        #endregion

        #region Auxiliary Methods
        static Update GetGoodUpdate(long chatId, string text) => new Update
        {

            Message = new Message
            {
                Chat = new Chat
                {
                    Id = chatId,
                },
                Text = text,
            }
        };

        void VerifyLoggerMessage(string expectedPart) => _loggerMock.Verify(logger => logger.Log(
        It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
        It.Is<EventId>(eventId => eventId.Id == 0),
        It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains(expectedPart)),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        #endregion
    }
}
