using AutoMapper;
using CB.Accessors.Implementations;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CB.Tests.Accessors;

public class GuildAccessorTests
{
    private readonly IMapper _mapper;
    private readonly DbContextOptions<CbContext> _dbOptions;

    public GuildAccessorTests()
    {
        var loggerFactory = NullLoggerFactory.Instance;
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<CbProfile>();
        }, loggerFactory);
        _mapper = config.CreateMapper();

        // Setup InMemory DbContext options
        _dbOptions = new DbContextOptionsBuilder<CbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
    }

    [Fact]
    public async Task CreateAsync_ShouldAddGuild()
    {
        await using var context = new CbContext(_dbOptions);

        var accessor = new GuildAccessor(context, _mapper);
        var guild = new Guild { Id = "guild1", DisplayName = "Test Guild" };

        var result = await accessor.CreateAsync(guild);

        Assert.NotNull(result);
        Assert.Equal("Test Guild", result.DisplayName);
        Assert.NotEqual(default, guild.CreatedDate);
        Assert.NotEqual(default, guild.ModifiedDate);

        var dbGuild = await context.Guilds.FindAsync("guild1");
        Assert.NotNull(dbGuild);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsGuild_WhenExists()
    {
        await using var context = new CbContext(_dbOptions);
        context.Guilds.Add(new Guild { Id = "guild2", DisplayName = "Guild 2" });
        await context.SaveChangesAsync();

        var accessor = new GuildAccessor(context, _mapper);

        var result = await accessor.GetByIdAsync("guild2");

        Assert.NotNull(result);
        Assert.Equal("Guild 2", result.DisplayName);
    }
}