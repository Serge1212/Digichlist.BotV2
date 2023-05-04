namespace Digichlist.Bot.Repositories
{
    /// <summary>
    /// The dedicated repo for working with ongoing command tasks.
    /// </summary>
    public class CommandTaskRepository : ICommandTaskRepository
    {
        readonly DigichlistContext _context;

        public CommandTaskRepository(DigichlistContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<CommandTask> GetAsync(long chatId) => await _context.CommandTasks.FirstOrDefaultAsync(cti => cti.ChatId == chatId && cti.ClosedAt == null);

        /// <inheritdoc />
        public async Task AddAsync(CommandTask commandTaskInfo)
        {
            await _context.AddAsync(commandTaskInfo);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(CommandTask commandTask)
        {
            _context.Update(commandTask);
            await _context.SaveChangesAsync();
        }
    }
}
