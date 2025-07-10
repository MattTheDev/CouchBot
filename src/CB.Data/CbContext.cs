using CB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CB.Data;

public class CbContext(DbContextOptions<CbContext> options) : DbContext(options)
{
    public DbSet<Creator> Creators => Set<Creator>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Users table
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<User>()
            .Property(p => p.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<User>()
            .Property(p => p.ModifiedDate)
            .HasColumnType("timestamp without time zone");

        // Configure Guilds table
        modelBuilder.Entity<Guild>().ToTable("Guilds");

        modelBuilder.Entity<Guild>()
            .Property(p => p.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Guild>()
            .Property(p => p.ModifiedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Guild>()
            .HasOne(g => g.User)
            .WithMany(u => u.Guilds)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure Creators table
        modelBuilder.Entity<Creator>().ToTable("Creators");

        modelBuilder.Entity<Creator>()
            .Property(p => p.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Creator>()
            .Property(p => p.ModifiedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Creator>()
            .HasOne(g => g.User)
            .WithMany(u => u.Creators)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}