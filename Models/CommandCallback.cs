namespace Digichlist.Bot.Models
{
    /// <summary>
    /// The model that contains info when user makes callback query.
    /// </summary>
    public class CommandCallback
    {
        /// <summary>
        /// The command this callback originates from.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The realted defect identifier.
        /// </summary>
        public int DefectId { get; set; }

        /// <summary>
        /// The desired status for the specified defect.
        /// Null means:
        /// Either user hasn't yet a chance to select the desired status,
        /// or this command does not involve statuses.
        /// </summary>
        public DefectStatus? Status { get; set; }
    }
}
