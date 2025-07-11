﻿using CB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CB.Data;

public class CbContext(DbContextOptions<CbContext> options) : DbContext(options)
{
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Creator> Creators => Set<Creator>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure AllowConfiguration table
        modelBuilder.Entity<ChannelConfiguration>().ToTable("AllowConfigurations");

        // Configure Channel
        modelBuilder.Entity<Channel>().ToTable("Channels");

        modelBuilder.Entity<Channel>()
            .Property(x => x.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Channel>()
            .Property(x => x.ModifiedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Channel>()
            .HasOne(x => x.Guild)
            .WithMany(x => x.Channels)
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure ChannelConfiguration table
        modelBuilder.Entity<ChannelConfiguration>().ToTable("ChannelConfigurations");

        modelBuilder.Entity<ChannelConfiguration>()
            .HasOne(cc => cc.GreetingChannel)
            .WithMany()
            .HasForeignKey(cc => cc.GreetingChannelId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ChannelConfiguration>()
            .HasOne(cc => cc.GoodbyeChannel)
            .WithMany()
            .HasForeignKey(cc => cc.GoodbyeChannelId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ChannelConfiguration>()
            .HasOne(cc => cc.LiveChannel)
            .WithMany()
            .HasForeignKey(cc => cc.LiveChannelId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ChannelConfiguration>()
            .HasOne(cc => cc.DiscordLiveChannel)
            .WithMany()
            .HasForeignKey(cc => cc.DiscordLiveChannelId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Creators table
        modelBuilder.Entity<Creator>().ToTable("Creators");

        modelBuilder.Entity<Creator>()
            .Property(x => x.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Creator>()
            .Property(x => x.ModifiedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Creator>()
            .HasOne(x => x.User)
            .WithMany(x => x.Creators)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure Guilds table
        modelBuilder.Entity<Guild>().ToTable("Guilds");

        modelBuilder.Entity<Guild>()
            .Property(x => x.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Guild>()
            .Property(x => x.ModifiedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Guild>()
            .HasOne(x => x.ChannelConfiguration)
            .WithOne(x => x.Guild)
            .HasForeignKey<ChannelConfiguration>(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Guild>()
            .HasOne(x => x.AllowConfiguration)
            .WithOne(x => x.Guild)
            .HasForeignKey<AllowConfiguration>(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Guild>()
            .HasOne(x => x.GuildConfiguration)
            .WithOne(x => x.Guild)
            .HasForeignKey<GuildConfiguration>(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Guild>()
            .HasOne(x => x.MessageConfiguration)
            .WithOne(x => x.Guild)
            .HasForeignKey<MessageConfiguration>(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Guild>()
            .HasOne(x => x.RoleConfiguration)
            .WithOne(x => x.Guild)
            .HasForeignKey<RoleConfiguration>(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure GuildConfiguration table
        modelBuilder.Entity<GuildConfiguration>().ToTable("GuildConfigurations");

        // Configure MessageConfiguration table
        modelBuilder.Entity<MessageConfiguration>().ToTable("MessageConfigurations");

        // Configure RoleConfiguration table
        modelBuilder.Entity<RoleConfiguration>().ToTable("RoleConfigurations");

        // Configure Users table
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<User>()
            .Property(x => x.CreatedDate)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<User>()
            .Property(x => x.ModifiedDate)
            .HasColumnType("timestamp without time zone");
    }
}