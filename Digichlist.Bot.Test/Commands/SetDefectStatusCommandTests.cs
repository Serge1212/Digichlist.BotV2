using Digichlist.Bot.Commands;
using Digichlist.Bot.Configuration;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models;
using Digichlist.Bot.Models.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Digichlist.Bot.Test.Commands
{
    public class SetDefectStatusCommandTests
    {
        readonly Mock<ITelegramBotClient> _botClientMock = new();
        readonly Mock<IUserRepository> _userRepoMock = new();
        readonly Mock<IDefectRepository> _defectRepoMock = new();
        readonly Mock<ICommandTaskRepository> _commandTaskRepoMock = new();
        readonly Mock<ILogger<SetDefectStatusCommand>> _loggerMock = new();
        readonly SetDefectStatusCommand _command;

        public SetDefectStatusCommandTests()
        {
            _command = new SetDefectStatusCommand(
                _botClientMock.Object,
                _userRepoMock.Object,
                _defectRepoMock.Object,
                _commandTaskRepoMock.Object,
                _loggerMock.Object
                );
        }

        #region Test Helpers
        static List<Defect> GetDefects() => new List<Defect>
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
        #endregion

        [Fact]
        public async Task ProcessAsync_ShouldSaveInitProps_OnSuccessFlow()
        {
            // Arrange.
            var chatId = 123847892374;
            var expectedMessagePart = "the defect to change its status";

            _commandTaskRepoMock
                .Setup(ctr => ctr.GetAsync(chatId))
                .ReturnsAsync(() => null);

            _userRepoMock
                .Setup(ur => ur.GetByChatIdAsync(chatId))
                .ReturnsAsync(() => new User
                {
                    ChatId = chatId,
                    Role = new Role
                    {
                        CanBeAssigned = true,
                    }
                });

            _defectRepoMock
                .Setup(dr => dr.GetManyByChatId(chatId))
                .Returns(GetDefects());

            var message = new BotMessage
            {
                ChatId = chatId,
                Text = BotCommands.SET_DEFECT_STATUS,
                MessageId = It.IsAny<int>(),
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _commandTaskRepoMock.Verify(ctr => ctr.AddAsync(
                It.Is<CommandTask>(ct =>
                ct.ChatId == chatId &&
                ct.CommandName == BotCommands.SET_DEFECT_STATUS &&
                ct.ExpiresAt > DateTime.Now)), Times.Once);

            _botClientMock.Verify(bc => bc.MakeRequestAsync(It.Is<SendMessageRequest>(m =>
            m.ChatId == chatId &&
            m.Text.Contains(expectedMessagePart) &&
            m.ReplyMarkup != null), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldSaveInitProps_OnSettledPrevious_CommandTask()
        {
            // Arrange.
            var chatId = 123847892374;
            var expectedMessagePart = "the defect to change its status";
            var expiredDatetime = DateTime.Now.AddMinutes(-5);

            _commandTaskRepoMock
                .Setup(ctr => ctr.GetAsync(chatId))
                .ReturnsAsync(() => new CommandTask
                {
                    Id = 1,
                    ChatId = chatId,
                    CommandName = BotCommands.SET_DEFECT_STATUS,
                    ExpiresAt = expiredDatetime,
                    ClosedAt = null,
                });

            _userRepoMock
                .Setup(ur => ur.GetByChatIdAsync(chatId))
                .ReturnsAsync(() => new User
                {
                    ChatId = chatId,
                    Role = new Role
                    {
                        CanBeAssigned = true,
                    }
                });

            _defectRepoMock
                .Setup(dr => dr.GetManyByChatId(chatId))
                .Returns(GetDefects());

            var message = new BotMessage
            {
                ChatId = chatId,
                Text = BotCommands.SET_DEFECT_STATUS,
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _commandTaskRepoMock.Verify(ctr => ctr.AddAsync(
                It.Is<CommandTask>(ct =>
                ct.ChatId == chatId &&
                ct.CommandName == BotCommands.SET_DEFECT_STATUS &&
                ct.ExpiresAt > DateTime.Now)), Times.Once);

            _botClientMock.Verify(bc => bc.MakeRequestAsync(It.Is<SendMessageRequest>(m =>
            m.ChatId == chatId &&
            m.Text.Contains(expectedMessagePart) &&
            m.ReplyMarkup != null), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldNotProcess_OnUndone_OngoingCommandTasks()
        {
            // Arrange.
            var chatId = 123847892374;
            var expectedMessagePart = "You have uncompleted commands that cannot be continued";
            var commandName = BotCommands.SET_DEFECT_STATUS;
            var expiredDatetime = DateTime.Now.AddMinutes(5);

            _commandTaskRepoMock
                .Setup(ctr => ctr.GetAsync(chatId))
                .ReturnsAsync(() => new CommandTask
                {
                    Id = 1,
                    ChatId = chatId,
                    CommandName = commandName,
                    ExpiresAt = expiredDatetime,
                    ClosedAt = null,
                });

            _userRepoMock
                .Setup(ur => ur.GetByChatIdAsync(chatId))
                .ReturnsAsync(() => new User
                {
                    ChatId = chatId,
                    Role = new Role
                    {
                        CanBeAssigned = true,
                    }
                });

            _defectRepoMock
                .Setup(dr => dr.GetManyByChatId(chatId))
                .Returns(GetDefects());

            var message = new BotMessage
            {
                ChatId = chatId,
                Text = BotCommands.SET_DEFECT_STATUS,
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == chatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }

        [Fact]
        public async Task ProcessAsync_ShouldNotProcess_OnInvalidRole()
        {
            // Arrange.
            var chatId = 123847892374;
            var expectedMessagePart = "your role does not have a permission";

            _commandTaskRepoMock
                .Setup(ctr => ctr.GetAsync(chatId))
                .ReturnsAsync(() => null);

            _userRepoMock
                .Setup(ur => ur.GetByChatIdAsync(chatId))
                .ReturnsAsync(() => new User
                {
                    ChatId = chatId,
                    Role = new Role
                    {
                        CanBeAssigned = false,
                    }
                });

            var message = new BotMessage
            {
                ChatId = chatId,
                Text = BotCommands.SET_DEFECT_STATUS,
            };

            // Act.
            await _command.ProcessAsync(message, CancellationToken.None);

            // Assert.
            _botClientMock.Verify(b => b.MakeRequestAsync(It.Is<SendMessageRequest>(m => m.ChatId == chatId && m.Text.Contains(expectedMessagePart)), CancellationToken.None));
        }
    }
}
