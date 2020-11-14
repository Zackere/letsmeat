using LetsMeatAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsMeatAPI {
  public class LMDbContext : DbContext {
    public LMDbContext(DbContextOptions<LMDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
  }
}
