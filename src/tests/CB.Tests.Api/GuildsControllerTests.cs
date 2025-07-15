using CB.Accessors.Contracts;
using CB.Api.Controllers;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CB.Tests.Api
{
    public class GuildsControllerTests
    {
        private readonly Mock<IGuildAccessor> _mockAccessor;
        private readonly GuildsController _controller;

        public GuildsControllerTests()
        {
            _mockAccessor = new Mock<IGuildAccessor>();
            _controller = new GuildsController(_mockAccessor.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithGuildList()
        {
            // Arrange
            var guilds = new List<GuildDto>
        {
            new GuildDto { Id = "1", DisplayName = "Guild One" },
            new GuildDto { Id = "2", DisplayName = "Guild Two" },
        };
            _mockAccessor.Setup(a => a.GetAllAsync()).ReturnsAsync(guilds);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedGuilds = Assert.IsAssignableFrom<List<GuildDto>>(okResult.Value);
            Assert.Equal(2, returnedGuilds.Count);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkWithGuild()
        {
            var guild = new GuildDto { Id = "123", DisplayName = "Test Guild" };
            _mockAccessor.Setup(a => a.GetByIdAsync("123")).ReturnsAsync(guild);

            var result = await _controller.GetById("123");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedGuild = Assert.IsType<GuildDto>(okResult.Value);
            Assert.Equal("123", returnedGuild.Id);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _mockAccessor.Setup(a => a.GetByIdAsync("notfound")).ReturnsAsync((GuildDto?)null);

            var result = await _controller.GetById("notfound");

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var guild = new Guild { Id = "newguild", DisplayName = "New Guild" };
            var guildDto = new GuildDto { Id = "newguild", DisplayName = "New Guild" };

            _mockAccessor.Setup(a => a.CreateAsync(guild)).ReturnsAsync(guildDto);

            var result = await _controller.Create(guild);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedGuild = Assert.IsType<GuildDto>(createdAtActionResult.Value);
            Assert.Equal("newguild", returnedGuild.Id);
            Assert.Equal("New Guild", returnedGuild.DisplayName);
            Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
            Assert.Equal("newguild", createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Update_ExistingGuild_ReturnsOkWithUpdatedGuild()
        {
            var updatedGuild = new Guild { DisplayName = "Updated Name", OwnerId = "owner1" };
            var updatedDto = new GuildDto { Id = "123", DisplayName = "Updated Name" };

            _mockAccessor.Setup(a => a.UpdateAsync("123", updatedGuild)).ReturnsAsync(updatedDto);

            var result = await _controller.Update("123", updatedGuild);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedGuild = Assert.IsType<GuildDto>(okResult.Value);
            Assert.Equal("Updated Name", returnedGuild.DisplayName);
        }

        [Fact]
        public async Task Update_NonExistingGuild_ReturnsNotFound()
        {
            var updatedGuild = new Guild { DisplayName = "Name" };

            _mockAccessor.Setup(a => a.UpdateAsync("notfound", updatedGuild)).ReturnsAsync((GuildDto?)null);

            var result = await _controller.Update("notfound", updatedGuild);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ExistingGuild_ReturnsNoContent()
        {
            _mockAccessor.Setup(a => a.DeleteAsync("123")).ReturnsAsync(true);

            var result = await _controller.Delete("123");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingGuild_ReturnsNotFound()
        {
            _mockAccessor.Setup(a => a.DeleteAsync("notfound")).ReturnsAsync(false);

            var result = await _controller.Delete("notfound");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}