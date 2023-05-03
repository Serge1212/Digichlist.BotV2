namespace Digichlist.Bot.Interfaces
{
    /// <summary>
    /// Dedicated repo for defect related functionality.
    /// </summary>
    public interface IDefectRepository
    {
        /// <summary>
        /// Gets a user's assigned defects by specified chat identifier.
        /// </summary>
        IEnumerable<Defect> GetDefectsByChatId(long chatId);

        /// <summary>
        /// Saves a brand new defect.
        /// </summary>
        Task SaveAsync(Defect defect);
    }
}
