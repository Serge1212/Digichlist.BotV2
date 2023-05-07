namespace Digichlist.Bot.Interfaces
{
    /// <summary>
    /// Dedicated repo for defect related functionality.
    /// </summary>
    public interface IDefectRepository
    {
        /// <summary>
        /// Gets a user's assigned defect by specified chat and defect identifiers.
        /// </summary>
        Task<Defect> GetSingleAsync(int defectId);

        /// <summary>
        /// Gets a user's assigned defects by specified chat identifier.
        /// </summary>
        IEnumerable<Defect> GetManyByChatId(long chatId);

        /// <summary>
        /// Saves a brand new defect.
        /// </summary>
        Task AddAsync(Defect defect);

        /// <summary>
        /// Updates existing defect.
        /// </summary>
        Task UpdateAsync(Defect defect);
    }
}
