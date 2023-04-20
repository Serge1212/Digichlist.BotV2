using User = Digichlist.Bot.Models.Entities.User;

namespace Digichlist.Bot.Context
{
    /// <summary>
    /// The db context of digichlist.
    /// </summary>
    public class DigichlistContext : DbContext
    {
        /// <summary>
        /// The db set that reflects the state of Users table.
        /// </summary>
        public DbSet<User>? Users { get; set; }

        /// <summary>
        /// The db set that reflects the state of Roles table.
        /// </summary>
        public DbSet<Role>? Roles { get; set; }

        /// <summary>
        /// The db set that reflects the state of Defects table.
        /// </summary>
        public DbSet<Defect>? Defects { get; set; }

        /// <summary>
        /// The db set that reflects the state of DefectImages table.
        /// </summary>
        public DbSet<DefectImage>? DefectImages { get; set; }

        /// <summary>
        /// Initializes the digichlist db context.
        /// </summary>
        /// <param name="options"></param>
        public DigichlistContext(DbContextOptions<DigichlistContext> options) : base(options)
        {
        }
    }
}
