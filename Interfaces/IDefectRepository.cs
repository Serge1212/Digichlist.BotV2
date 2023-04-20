namespace Digichlist.Bot.Interfaces
{
    /// <summary>
    /// Dedicated repo for defect related functionality.
    /// </summary>
    public interface IDefectRepository
    {
        Task SaveAsync(Defect defect);
    }
}
