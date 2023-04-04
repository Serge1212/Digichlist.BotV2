namespace Digichlist.Bot.Models.Entities
{
    /// <summary>
    /// The entity that represents a role that a user can have.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// The unique identifier of the role.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The flag that indicates if this role can add defects.
        /// </summary>
        public bool CanAdd { get; set; }

        /// <summary>
        /// The flag that indicates if a user with this role can be assigned to fix defects.
        /// </summary>
        public bool CanBeAssigned { get; set; }

        /// <summary>
        /// The users that have this role.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();
    }
}
