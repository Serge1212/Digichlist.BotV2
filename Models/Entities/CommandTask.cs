namespace Digichlist.Bot.Models.Entities
{
    /// <summary>
    /// Contains information about ongoing task that is split by steps.
    /// Is essentially a context for a sophisticated flows.
    /// </summary>
    public class CommandTask
    {
        /// <summary>
        /// The command task unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Related command name for this task.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// The related user's chat identifier.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// The defect's unique identifier.
        /// Null means that there is no yet specified defect.
        /// </summary>
        public int? DefectId { get; set; }

        /// <summary>
        /// The related defect's status signifier.
        /// Null means that no status signifier is yet defined.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// The expiration date and time for this command task.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// The date and time this command task was closed.
        /// Null means that this command task is not yet closed.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// Related defect info.
        /// </summary>
        public Defect? Defect { get; set; }
    }
}
