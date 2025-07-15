using CB.Accessors.Contracts;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CB.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuildsController(IGuildAccessor guildAccessor) : ControllerBase
{
    // GET api/guilds
    [HttpGet]
    public async Task<ActionResult<List<GuildDto>>> GetAll()
    {
        var guilds = await guildAccessor
            .GetAllAsync()
            .ConfigureAwait(false);
        return Ok(guilds);
    }

    // GET api/guilds/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GuildDto?>> GetById(string id)
    {
        var guild = await guildAccessor
            .GetByIdAsync(id)
            .ConfigureAwait(false);
        if (guild == null)
        {
            return NotFound();
        }

        return Ok(guild);
    }

    // POST api/guilds
    [HttpPost]
    public async Task<ActionResult<GuildDto>> Create([FromBody] Guild newGuild)
    {
        var created = await guildAccessor.CreateAsync(newGuild);

        // Return 201 Created with location header for the new resource
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/guilds/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<GuildDto?>> Update(string id, [FromBody] Guild updatedGuild)
    {
        var updated = await guildAccessor.UpdateAsync(id, updatedGuild);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    // DELETE api/guilds/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var deleted = await guildAccessor.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}