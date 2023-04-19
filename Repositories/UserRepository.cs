using Digichlist.Bot.Context;
using Digichlist.Bot.Interfaces;
using Digichlist.Bot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Digichlist.Bot.Repositories
{
    /// <summary>
    /// Dedicated repo for user related functionality.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        readonly DigichlistContext _context;

        public UserRepository(DigichlistContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<User?> GetByIdAsync(long chatId) => await _context.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);

        public async Task SaveUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
