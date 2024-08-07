using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>()
          .HasOne(u => u.UserProfile)
          .WithOne(p => p.User)
          .HasForeignKey<UserProfile>(p => p.UserId);
    }
  }
}
