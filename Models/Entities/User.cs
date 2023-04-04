namespace Digichlist.Bot.Models.Entities
{
    /// <summary>
    /// The entity that represents the end-user info.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The telegram chat identifier.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The flag that indicates that the user is registered or not.
        /// </summary>
        public bool IsRegistered { get; set; }

        /// <summary>
        /// The user's role.
        /// Null when none.
        /// </summary>
        public int? RoleId { get; set; }
        //public Role Role { get; set; }
        //public List<Defect> Defects { get; set; } = new List<Defect>();
        //public List<AssignedDefect> AssignedDefects { get; set; } = new List<AssignedDefect>();
    }
}
