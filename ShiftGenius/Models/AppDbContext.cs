using Microsoft.EntityFrameworkCore;
using ShiftGeniusLibDB.Models;

namespace ShiftGenius.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TimeOffRequest> TimeOffRequests { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
