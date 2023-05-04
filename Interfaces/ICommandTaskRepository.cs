namespace Digichlist.Bot.Interfaces
{
    /// <summary>
    /// The dedicated repo for working with ongoing command tasks.
    /// </summary>
    public interface ICommandTaskRepository
    {
        /// <summary>
        /// Returns a single command task for specified chat identifier.
        /// </summary>
        Task<CommandTask> GetAsync(long chatId);

        /// <summary>
        /// Saves the initial poperties for a brand new command task.
        /// </summary>
        Task AddAsync(CommandTask commandTaskInfo);

        /// <summary>
        /// Pathes command task related info.
        /// </summary>
        Task UpdateAsync(CommandTask commandTask);
    }
}
