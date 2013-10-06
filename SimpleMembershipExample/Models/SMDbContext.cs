using System.Data.Entity;

namespace SimpleMembershipExample.Models
{
    public class SMDbContext : DbContext
    {
        public SMDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}