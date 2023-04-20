using User = Digichlist.Bot.Models.Entities.User;

namespace Digichlist.Bot.Interfaces
{
    /// <summary>
    /// Dedicated repo for user related functionality.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the user info by specified identifier.
        /// </summary>
        Task<User?> GetByChatIdAsync(long chatId);

        /// <summary>
        /// Saves a brand new user.
        /// </summary>
        Task SaveUserAsync(User user);
    }
}
