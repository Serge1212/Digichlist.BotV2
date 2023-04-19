using Digichlist.Bot.Models;

namespace Digichlist.Bot.Interfaces
{
    /// <summary>
    /// Generalized command contract.
    /// </summary>
    public interface IBotCommand
    {
        /// <summary>
        /// Starts processing the actual commands.
        /// </summary>
        Task ProcessAsync(BotMessage message, CancellationToken cancellationToken);
    }
}
