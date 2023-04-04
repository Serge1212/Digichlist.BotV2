using System.ComponentModel.DataAnnotations;

namespace Digichlist.Bot.Models.Entities
{
    /// <summary>
    /// The entity that represents the defect.
    /// </summary>
    public class Defect
    {
        /// <summary>
        /// The unique identifier of the defect.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The number of the room where the defect is spotted.
        /// </summary>
        [Required]
        public int RoomNumber { get; set; }

        /// <summary>
        /// The description of the defect.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// The date the defect was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The publisher user.
        /// Null when none.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// The information about the user that published this defect.
        /// </summary>
        public User Publisher { get; set; }

        /// <summary>
        /// The detailed workflow information about this defect.
        /// Not null only when it's assigned.
        /// </summary>
        public AssignedDefect? AssignedDefect { get; set; }

        /// <summary>
        /// The related images.
        /// </summary>
        public List<DefectImage> DefectImages { get; set; } = new();
    }
}
