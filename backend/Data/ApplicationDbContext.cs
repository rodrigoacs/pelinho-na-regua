using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public DbSet<User> Users { get; set; }
  }
}
