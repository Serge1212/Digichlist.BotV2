using Digichlist.Bot.Enums;

namespace Digichlist.Bot.Models.Entities
{
    /// <summary>
    /// The entity that represents the assigned defect.
    /// </summary>
    public class AssignedDefect
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date and time the defect was closed.
        /// Has value only when the status is <seealso cref="DefectStatus.Eliminated"/>.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// The date and time the defect status was changed.
        /// </summary>
        public DateTime? StatusChangedAt { get; set; }

        /// <summary>
        /// The actual defect identifier.
        /// </summary>
        public int DefectId { get; set; }

        /// <summary>
        /// The information about the defect.
        /// </summary>
        public Defect? Defect { get; set; }

        /// <summary>
        /// The status of the defect.
        /// </summary>
        public DefectStatus Status { get; set; }

        /// <summary>
        /// The assigned worker identifier.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// More information about the assigned worker.
        /// </summary>
        public User? AssignedWorker { get; set; }
    }
}
