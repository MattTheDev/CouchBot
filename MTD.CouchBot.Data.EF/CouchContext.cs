using Microsoft.EntityFrameworkCore;
using MTD.CouchBot.Domain.Models.Bot;
using System;

namespace MTD.CouchBot.Data.EF
{
    public class CouchContext : DbContext
    {
        public DbSet<DiscordServer> DiscordServers { get; set; }

        public CouchContext(DbContextOptions<CouchContext> contextOptions)
            : base(contextOptions) { }
    }
}
