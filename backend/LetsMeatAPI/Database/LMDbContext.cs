using LetsMeatAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsMeatAPI {
  public class LMDbContext : DbContext {
    public LMDbContext(DbContextOptions<LMDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Debt> Debts { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<User>()
        .HasMany(user => user.DebtsForMe)
        .WithOne(debt => debt.To)
        .OnDelete(DeleteBehavior.NoAction);
      modelBuilder.Entity<User>()
        .HasMany(user => user.DebtsForOthers)
        .WithOne(debt => debt.From)
        .OnDelete(DeleteBehavior.NoAction);
      modelBuilder.Entity<User>()
        .HasMany(user => user.Invitations)
        .WithOne(inv => inv.To)
        .OnDelete(DeleteBehavior.NoAction);
      modelBuilder.Entity<User>()
        .HasMany(user => user.OwnedGroups)
        .WithOne(g => g.Owner)
        .OnDelete(DeleteBehavior.NoAction);
      modelBuilder.Entity<User>()
        .HasMany(user => user.CreatedEvents)
        .WithOne(ev => ev.Creator)
        .OnDelete(DeleteBehavior.NoAction);
      modelBuilder.Entity<Debt>()
        .HasKey(debt => new { debt.FromId, debt.ToId, debt.GroupId });
      modelBuilder.Entity<Vote>()
        .HasKey(vote => new { vote.EventId, vote.UserId });
      modelBuilder.Entity<Invitation>()
        .HasKey(inv => new { inv.ToId, inv.GroupId });
      modelBuilder.Entity<Group>()
        .HasMany(g => g.Users)
        .WithMany(u => u.Groups);
    }
  }
}
