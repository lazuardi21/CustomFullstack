using Microsoft.EntityFrameworkCore;
using SportEventsAPI.Models;

namespace SportEventsAPI.Data
{
    namespace SportEventsAPI.Data
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Organizer> Organizers { get; set; }
            public DbSet<SportEvent> SportEvents { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Configure entity filters, keys, relationships, etc.
            }
        }
    }


}
