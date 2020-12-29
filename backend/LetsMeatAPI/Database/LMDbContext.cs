using LetsMeatAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsMeatAPI {
  public class LMDbContext : DbContext {
    public LMDbContext(DbContextOptions<LMDbContext> options) : base(options) { }
    public DbSet<CustomLocation> CustomLocations { get; set; }
    public DbSet<Debt> Debts { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<GoogleMapsLocation> GoogleMapsLocations { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<PendingDebt> PendingDebts { get; set; }
    public DbSet<DebtFromImage> DebtsFromImages { get; set; }
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
        .HasMany(user => user.PendingDebtsForMe)
        .WithOne(debt => debt.To)
        .OnDelete(DeleteBehavior.NoAction);
      modelBuilder.Entity<User>()
        .HasMany(user => user.PendingDebtsForOthers)
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
      modelBuilder.Entity<User>()
        .HasMany(user => user.UploadedImages)
        .WithOne(i => i.UploadedBy)
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
      modelBuilder.Entity<Group>()
        .HasMany(g => g.CustomLocations)
        .WithOne(l => l.CreatedFor)
        .OnDelete(DeleteBehavior.ClientSetNull);
      modelBuilder.Entity<Group>()
        .HasMany(g => g.Images)
        .WithOne(i => i.Group)
        .OnDelete(DeleteBehavior.ClientSetNull);
      modelBuilder.Entity<Group>()
        .HasMany(g => g.PendingDebts)
        .WithOne(debt => debt.Group)
        .OnDelete(DeleteBehavior.ClientSetNull);
      modelBuilder.Entity<Event>()
        .HasMany(e => e.CandidateCustomLocations)
        .WithMany(l => l.EventsWithMe);
      modelBuilder.Entity<Event>()
        .HasMany(e => e.CandidateGoogleMapsLocations)
        .WithMany(l => l.EventsWithMe);
      modelBuilder.Entity<Event>()
        .HasMany(e => e.Images)
        .WithOne(i => i.Event)
        .OnDelete(DeleteBehavior.SetNull);
      modelBuilder.Entity<Event>()
        .HasMany(e => e.PendingDebts)
        .WithOne(debt => debt.Event)
        .OnDelete(DeleteBehavior.SetNull);
      modelBuilder.Entity<Image>()
        .HasMany(i => i.PendingDebtsWithMe)
        .WithOne(debt => debt.Image)
        .OnDelete(DeleteBehavior.SetNull);
    }
  }
}
